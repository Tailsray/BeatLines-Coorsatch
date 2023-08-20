using Godot;

public partial class DebugFrameCounter : Label
{
	public override void _Process(double delta)
	{
		Text = Engine.GetFramesPerSecond().ToString();
	}

	public void ToggleVisibility()
	{
		Visible = !Visible;
	}
}