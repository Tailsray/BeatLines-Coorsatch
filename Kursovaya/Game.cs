using Godot;

public partial class Game : Node
{
	StateMachine SM;
	private retryButton _retryButton = new retryButton();
	public override void _Ready()
	{
		SM = GetNode<StateMachine>("Conductor/StateMachine");
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _UnhandledInput(InputEvent ev)
	{
		if (Input.IsActionJustPressed("exit"))
			GetTree().Quit();
		
		if (Input.IsActionJustPressed("restart"))
			GetTree().ReloadCurrentScene();

		if (Input.IsActionJustPressed("pause"))
		{
			GetNode<CanvasModulate>("pauseBackground").Visible = !GetNode<CanvasModulate>("pauseBackground").Visible;
			GetNode<retryButton>("retryButton").Visible = !GetNode<retryButton>("retryButton").Visible;
			GetTree().Paused = !(GetTree().Paused);
		}

		if (Input.IsActionJustPressed("debug_jump"))
		{
			var jump = new JumpToBeat();
			jump.Position = new Vector2(350, 100);
			AddChild(jump);
			//GetTree().Paused = true;
		}

		
	}
}
