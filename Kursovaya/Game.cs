using Godot;

public partial class Game : Node
{
	[Signal]
	public delegate void PauseButtonPressedEventHandler();

	StateMachine SM;
	PackedScene condRes;
	Conductor cond;

	public int Speed { get; set; }

	public override void _Ready()
	{
		GD.Print("Current Scene: Game");
		condRes = ResourceLoader.Load<PackedScene>("res://Conductor.tscn");
		cond = condRes.Instantiate<Conductor>();
		AddChild(cond);
		SM = cond.GetNode<StateMachine>("StateMachine");
		SM.Speed = (105 - Mathf.Pow(11 - Speed, 2)) / 20;
		cond.GetNode<Label>("SpeedLabel").Text = $"Speed: {SM.Speed}";
		ProcessMode = ProcessModeEnum.Always;
	}

	public void OnResumePressed()
	{
		GetNode<PauseMenu>("PauseMenu").Visible = false;
		GetTree().Paused = false;
	}
	
	public void OnRetryPressed()
	{
		GetTree().GetFirstNodeInGroup("conductor").QueueFree();
		
		cond = condRes.Instantiate<Conductor>();
		AddChild(cond);
		SM = cond.GetNode<StateMachine>("StateMachine");
		SM.Speed = (105 - Mathf.Pow(11 - Speed, 2)) / 20;
		cond.GetNode<Label>("SpeedLabel").Text = $"Speed: {SM.Speed}";

		OnResumePressed();
	}

	public void OnRebindPressed()
	{
		var reb = ResourceLoader.Load<PackedScene>("res://RebindMenu.tscn").Instantiate<RebindMenu>();
		AddChild(reb);
	}

	public void OnExitPressed()
	{
		var SongSelectScene = ResourceLoader.Load<PackedScene>("res://SongSelect.tscn").Instantiate<SongSelect>();
		GetWindow().AddChild(SongSelectScene);
		QueueFree();
		GetTree().Paused = false;
	}

	public override void _UnhandledInput(InputEvent ev)
	{
		if (Input.IsActionJustPressed("pause") && !GetTree().Paused)
		{
			GetTree().Paused = true;
			GetNode<PauseMenu>("PauseMenu").Visible = true;
			EmitSignal(SignalName.PauseButtonPressed);
		}

		if (Input.IsActionJustPressed("toggle_frame_counter"))
			GetNode<DebugFrameCounter>("DebugFrameCounter").ToggleVisibility();
	}
}
