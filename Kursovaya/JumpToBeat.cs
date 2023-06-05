using Godot;
using System;

public partial class JumpToBeat : SpinBox
{
	public void _UnhandledInput(InputEventKey ev)
	{
		if (ev.Keycode == Key.Enter)
			Value = Single.Parse(GetLineEdit().Text);
	}
	public void OnValueChanged(float value)
	{
		GD.Print("hey");
		GetNode<MusicSource>("../Conductor/MusicSource").Seek(value);
		GetTree().Paused = false;
		QueueFree();
	}
}
