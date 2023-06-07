using Godot;
using System;

public partial class retryButton : TextureButton
{
	public override void _Ready()
	{
		Disabled = false;
	}
	public override void _Pressed()
	{
		GetParent<Game>().ReloadConductor(0);
	}
}
