using Godot;

public partial class Game : Node
{
	StateMachine SM;
	PackedScene condRes;
	Conductor cond;
	private retryButton _retryButton = new retryButton();

	public override void _Ready()
	{
		condRes = ResourceLoader.Load<PackedScene>("res://Conductor.tscn");
		cond = condRes.Instantiate<Conductor>();
		AddChild(cond);
		SM = GetNode<StateMachine>("Conductor/StateMachine");
		ProcessMode = ProcessModeEnum.Always;
	}

	async public void ReloadConductor(double startFrom)
	{
		GetTree().Paused = false;

		GetNode<ColorRect>("PauseBackground").Visible = false;
		GetNode<retryButton>("retryButton").Visible = false;

		GetTree().GetFirstNodeInGroup("conductor").QueueFree();
		cond = condRes.Instantiate<Conductor>();
		AddChild(cond);

		await ToSignal(GetTree().CreateTimer(1),
					   SceneTreeTimer.SignalName.Timeout);

		GetTree().GetFirstNodeInGroup("conductor").Call("JumpToBeat", startFrom);
	}

	public override void _UnhandledInput(InputEvent ev)
	{
		if (Input.IsActionJustPressed("exit"))
			GetTree().Quit();
		
		if (Input.IsActionJustPressed("restart"))
		{
			ReloadConductor(0);
		}

		if (Input.IsActionJustPressed("pause"))
		{
			GetNode<ColorRect>("PauseBackground").Visible = !GetNode<ColorRect>("PauseBackground").Visible;
			GetNode<retryButton>("retryButton").Visible = !GetNode<retryButton>("retryButton").Visible;
			GetTree().Paused = !(GetTree().Paused);
		}

		if (Input.IsActionJustPressed("debug_jump"))
		{
			GetTree().Paused = true;
			GetNode<JumpToBeat>("JumpToBeat").Visible = true;
		}
	}
}
