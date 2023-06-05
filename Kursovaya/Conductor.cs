using Godot;
using System;
using System.Collections.Generic;

public partial class Conductor : Node2D
{
	[Signal]
	public delegate void ChartIsReadEventHandler();

	int PathIDCounter { get; set; }
	public uint ExScore { get; set; }
	public uint MaxScore { get; set; }
	public uint NiceCount { get; set;}
	public uint OffCount { get; set;}
	public uint BadCount { get; set;}
	uint Score { get; set; }
	uint Combo { get; set; }
	string chartPath = "res://Charts/Stay3.chrt";
	List<List<string>> chart;
	StateMachine SM;
	InputProcessor IP;
	MusicSource MS;
	Label ScoreLabel;
	Label SpeedLabel;
	AudioStreamPlayer Clicker;
	APFCIndicator indicator;
	PackedScene noteScene;
	PackedScene pathScene;
	PackedScene splaScene;

	Godot.Collections.Array<Note> notes;

	async public override void _Ready()
	{
		noteScene = ResourceLoader.Load<PackedScene>("res://Note.tscn");
		pathScene = ResourceLoader.Load<PackedScene>("res://Path.tscn");
		splaScene = ResourceLoader.Load<PackedScene>("res://ComboSplash.tscn");

		ProcessMode = ProcessModeEnum.Pausable;

		SM = GetNode<StateMachine>("StateMachine");
		MS = GetNode<MusicSource>("MusicSource");
		ScoreLabel = GetNode<Label>("ScoreLabel");
		SpeedLabel = GetNode<Label>("SpeedLabel");
		Clicker = GetNode<AudioStreamPlayer>("Clicker");
		indicator = GetNode<APFCIndicator>("APFCIndicator");

		chart = new List<List<string>>();
		using var chartFile = FileAccess.Open(chartPath, FileAccess.ModeFlags.Read);
		while (chartFile.GetPosition() < chartFile.GetLength())
			chart.Add(new List<string>(chartFile.GetLine().Split(" ", 
								StringSplitOptions.RemoveEmptyEntries)));

		for (int i = 0; i < chart.Count; i++)
		{
			int index = Convert.ToInt32(chart[i][1]);

			if (chart[i][0] == "BPM")
				MS.BPM = Single.Parse(chart[i][1]);

			if (chart[i][0] == "P")
			{
				if (index > PathIDCounter)
				{
					SM.AddNewIndex();

					Path path = pathScene.Instantiate<Path>();
					AddChild(path);

					PathIDCounter++;
					path.MyID = PathIDCounter;
					path.InitStates(SM);
				}

				SM.AddState(Int32.Parse(chart[i][1]),
							new StateMachine.State(
								Double.Parse(chart[i][2]),
								Single.Parse(chart[i][3]),
								chart[i][4]));
								
			}

			if (chart[i][0] == "N")
			{
				Note note = noteScene.Instantiate<Note>();
				AddChild(note);

				note.NoteHit += OnNoteHit;

				note.Path1ID = index;

				if (chart[i].Count == 4)
				{
					note.Path2ID = Int32.Parse(chart[i][2]);
					note.MyTime = Double.Parse(chart[i][3]);
					note.TapsToGo = 2;
					MaxScore += 4;
				}
				else
				{
					note.Path2ID = 0;
					note.MyTime = Double.Parse(chart[i][2]);
					note.TapsToGo = 1;
					MaxScore += 2;
				}

				note.InitReferences(SM, MS);
			}
		}

		EmitSignal(SignalName.ChartIsRead);

		IP = GetNode<InputProcessor>("InputProcessor");
		IP.ChartOver += WinSplash;

		await ToSignal(GetTree().CreateTimer(1),
					   SceneTreeTimer.SignalName.Timeout);

		MS.Play();

		// DEBUG - DELETE LATER
		// MS.Seek(156 * 60f / MS.BPM);
	}

	public override void _UnhandledInput(InputEvent ev)
	{
		if (Input.IsActionJustPressed("speed_up"))
			SM.Speed += 0.5f;
		
		if (Input.IsActionJustPressed("speed_down"))
			SM.Speed -= 0.5f;

		SpeedLabel.Text = $"Speed: {SM.Speed}";
	}

	public void OnNoteHit(uint Grade, float pos)
	{
		if (Grade != 3)
			Clicker.Play();

		ExScore += Grade - (Grade == 3 ? 3u : 0);
		Score = 1000000 * ExScore / MaxScore;
		ScoreLabel.Text = Score.ToString();

		ComboSplash splash = splaScene.Instantiate<ComboSplash>();
		switch (Grade)
		{
			case 2:
				splash.Grade = "NICE";
				NiceCount++;
				break;
			case 1:
				splash.Grade = "OFF";
				OffCount++;
				break;
			case 3:
			case 0:
				splash.Grade = "BAD";
				BadCount++;
				break;
		}

		Combo++;
		if (Grade % 3 == 0)
			Combo = 0;
		splash.Combo = (Combo > 1 ? $" {Combo.ToString()}" : "");

		splash.Position = new Vector2(pos, 580f);

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

		Label Splash = new Label();
		if (Score == 1000000)
			DisplayCentered("ALL NICE!", Colors.Yellow, 320, 120, 20);
		else if (Score >= 700000)
			DisplayCentered("CLEARED!", Colors.White, 320, 120, 20);
		else
			DisplayCentered("FAILED...!", Colors.Crimson, 320, 120, 20);

		DisplayCentered($"NICE: {NiceCount}", Colors.SpringGreen, 500, 40, 12);
		DisplayCentered($"OFF: {OffCount}", Colors.DodgerBlue, 560, 40, 12);
		DisplayCentered($"BAD: {BadCount}", Colors.PaleVioletRed, 620, 40, 12);
	}
}