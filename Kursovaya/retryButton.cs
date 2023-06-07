using Godot;
using System;

public partial class retryButton : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public override void _Pressed()
	
	{
		Visible = false;
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
		GetNode<CanvasModulate>("pauseBackground").Visible = false;
	}
}
