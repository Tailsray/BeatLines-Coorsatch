using Godot;
using System;
using System.Collections.Generic;

public partial class Conductor : Node2D
{
	[Signal]
	public delegate void ChartIsReadEventHandler();

	int PathIDCounter { get; set; }
	int MaxScore { get; set; }
	int NiceCount { get; set;}
	int OffCount { get; set;}
	int BadCount { get; set;}
	int Score { get; set; }
	int Combo { get; set; }
	string chartPath = "res://Charts/Stay3.chrt";
	List<List<string>> chart;
	StateMachine SM;
	InputProcessor IP;
	MusicSource MS;
	ScoreTable ST;
	Label SpeedLabel;
	AudioStreamPlayer Clicker;
	APFCIndicator indicator;
	PackedScene noteScene;
	PackedScene pathScene;
	PackedScene splaScene;

	Godot.Collections.Array<Note> notes;

	async public override void _Ready()
	{
		AddToGroup("conductor");

		noteScene = ResourceLoader.Load<PackedScene>("res://Note.tscn");
		pathScene = ResourceLoader.Load<PackedScene>("res://Path.tscn");
		splaScene = ResourceLoader.Load<PackedScene>("res://ComboSplash.tscn");

		ProcessMode = ProcessModeEnum.Pausable;

		SM = GetNode<StateMachine>("StateMachine");
		MS = GetNode<MusicSource>("MusicSource");
		ST = GetNode<ScoreTable>("ScoreTable");
		SpeedLabel = GetNode<Label>("SpeedLabel");
		Clicker = GetNode<AudioStreamPlayer>("Clicker");
		indicator = GetNode<APFCIndicator>("APFCIndicator");

		chart = new List<List<string>>();
		using var chartFile = FileAccess.Open(chartPath, FileAccess.ModeFlags.Read);
		while (chartFile.GetPosition() < chartFile.GetLength())
			chart.Add(new List<string>(chartFile.GetLine().Split(" ", 
								StringSplitOptions.RemoveEmptyEntries)));

		foreach (var t in chart)
		{
			var index = AsInt(t[1]);

			if (t[0] == "BPM")
				MS.BPM = AsFloat(t[1]);

			if (t[0] == "P")
			{
				if (index > PathIDCounter)
				{
					SM.AddNewIndex();

					var path = pathScene.Instantiate<Path>();
					AddChild(path);

					PathIDCounter++;
					path.MyID = PathIDCounter;
					path.InitStates(SM);
				}

				SM.AddState(int.Parse(t[1]),
							new StateMachine.State(AsDouble(t[2]), AsFloat(t[3]), t[4]));
			}

			if (t[0] == "N")
			{
				var note = noteScene.Instantiate<Note>();
				AddChild(note);
				note.NoteHit += ST.OnNoteHit;
				note.NoteHit += OnNoteHit;

				if (t.Count == 4)
				{
					note.InitNote(SM, MS, index, AsInt(t[2]), AsDouble(t[3]), 2);
					MaxScore += 4;
				}
				else
				{
					note.InitNote(SM, MS, index, 0, AsDouble(t[2]), 1);
					MaxScore += 2;
				}
			}
		}

		ST.SetMaxScore(MaxScore);

		IP = GetNode<InputProcessor>("InputProcessor");
		IP.ChartOver += WinSplash;

		await ToSignal(GetTree().CreateTimer(1),
					   SceneTreeTimer.SignalName.Timeout);

		MS.Play();
	}

	int AsInt(string num)
	{
		return int.Parse(num);
	}

	float AsFloat(string num)
	{
		return float.Parse(num, System.Globalization.CultureInfo.InvariantCulture);
	}

	double AsDouble(string num)
	{
		return double.Parse(num, System.Globalization.CultureInfo.InvariantCulture);
	}

	public override void _Process(double delta)
	{
		ST.UpdateScore(MS.CurrentTime, Score);
	}

	public void JumpToBeat(double beat)
	{
		MS.Play((float)beat * 60f / MS.BPM);
	}

	public override void _UnhandledInput(InputEvent ev)
	{
		if (Input.IsActionJustPressed("speed_up"))
			SM.Speed += 0.5f;
		
		if (Input.IsActionJustPressed("speed_down"))
			SM.Speed -= 0.5f;

		SpeedLabel.Text = $"Speed: {SM.Speed}";
	}

	public void OnNoteHit(bool success, int dscore, double t, float pos)
	{
		if (success)
			Clicker.Play();

		Score += dscore;

		var splash = splaScene.Instantiate<ComboSplash>();
		switch (dscore)
		{
			case 2:
				splash.Grade = "NICE";
				NiceCount++;
				break;
			case 1:
				splash.Grade = "OFF";
				OffCount++;
				break;
			case 0:
				splash.Grade = "BAD";
				BadCount++;
				break;
		}

		Combo++;
		if (dscore == 0)
			Combo = 0;
		splash.Combo = (Combo > 1 ? $" {Combo.ToString()}" : "");

		splash.InitPos = new Vector2(pos, 560f);

		AddChild(splash);

		if (BadCount > 0)
			indicator.SetIndicators(0);
		else if (OffCount > 0)
			indicator.SetIndicators(1);
	}

	void DisplayCentered(string text, Color color, float posY, int fontSize, int outlineSize)
	{
		Label label = new Label();
		label.Text = text;
		label.AddThemeColorOverride("font_color", color);
		label.AddThemeColorOverride("font_outline_color", Colors.Black);
		label.AddThemeFontSizeOverride("font_size", fontSize);
		label.AddThemeConstantOverride("outline_size", outlineSize);
		label.Size = new Vector2(800, fontSize);
		label.Position = new Vector2(0, posY);
		label.HorizontalAlignment = HorizontalAlignment.Center;
		AddChild(label);
	}

	async public void WinSplash()
	{
		await ToSignal(GetTree().CreateTimer(1),
					   SceneTreeTimer.SignalName.Timeout);

		indicator.SetIndicators(0);

		
		switch (ST.Score())
		{
			case 1000000:
				DisplayCentered("ALL NICE!", Colors.Yellow, 320, 120, 20);
				break;
			case >= 700000:
				DisplayCentered("CLEARED!", Colors.White, 320, 120, 20);
				break;
			default:
				DisplayCentered("FAILED...!", Colors.Crimson, 320, 120, 20);
				break;
		}

		DisplayCentered($"NICE: {NiceCount}", Colors.SpringGreen, 500, 40, 12);
		DisplayCentered($"OFF: {OffCount}", Colors.DodgerBlue, 560, 40, 12);
		DisplayCentered($"BAD: {BadCount}", Colors.PaleVioletRed, 620, 40, 12);
	}
}
