using Godot;
using System;

public partial class JumpToBeat : SpinBox
{
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.WhenPaused;
	}

	public override void _Process(double delta)
	{
		
	}

	public override void _UnhandledInput(InputEvent ev)
	{
		if (ev is InputEventKey key && key.Keycode == Key.Enter)
			Value = float.Parse(GetLineEdit().Text);
	}

	void OnValueChanged(double value)
	{
		GetTree().Paused = false;
		GetParent<Game>().ReloadConductor(value);		
		Visible = false;
	}
}
