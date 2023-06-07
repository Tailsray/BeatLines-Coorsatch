using Godot;

public partial class InputProcessor : Node
{
	[Signal]
	public delegate void ChartOverEventHandler();

	public override void _UnhandledInput(InputEvent ev)
	{
		if (GetTree().GetNodesInGroup("notes").Count == 0)
		{
			EmitSignal(SignalName.ChartOver);
			QueueFree();
		}
		else
		{
			var nextNote = GetTree().GetNodesInGroup("notes")[0];

			if (Input.IsActionJustPressed("key1"))
				nextNote.Call("Pressed");
			if (Input.IsActionJustPressed("key2"))
				nextNote.Call("Pressed");
			if (Input.IsActionJustPressed("key3"))
				nextNote.Call("Pressed");
			if (Input.IsActionJustPressed("key4"))
				nextNote.Call("Pressed");
		}
	}
}