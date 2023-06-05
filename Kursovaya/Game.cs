using Godot;

public partial class Game : Node
{
	StateMachine SM;

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
			GetTree().Paused = !(GetTree().Paused);

		if (Input.IsActionJustPressed("debug_jump"))
		{
			//GetTree().Paused = true;
			JumpToBeat jump = new JumpToBeat();
			jump.Position = new Vector2(350, 100);
			AddChild(jump);
		}
	}
}
