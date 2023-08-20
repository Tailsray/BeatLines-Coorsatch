using Godot;

public partial class SongSelect : Control
{
	int Speed = 2;
	Label SpeedLabel;

    public override void _Ready()
    {
		GD.Print("Current Scene: Song Select");
        SpeedLabel = GetNode<Label>("SpeedLabel");
	}

	public override void _Process(double delta)
	{
		if (GetNodeOrNull<Label>("SpeedLabel") != null)
			SpeedLabel.Text = $"Speed: {Speed}\n\nUp/Down: Change speed\nEnter: Start\nEscape: Quit";
	}

    public override void _Input(InputEvent ev)
    {
        if (ev.IsActionPressed("start"))
		{
			var GameScene = ResourceLoader.Load<PackedScene>("res://Game.tscn").Instantiate<Game>();
			GameScene.Speed = Speed;
			GetWindow().AddChild(GameScene);
			QueueFree();
		}

		if (ev.IsActionPressed("pause"))
			GetTree().Quit();

		if (ev.IsActionPressed("up") && Speed < 10)
			Speed++;

		if (ev.IsActionPressed("down") && Speed > 1)
			Speed--;
    }
}
