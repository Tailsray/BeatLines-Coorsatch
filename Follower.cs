using Godot;

public partial class Follower : Node2D
{	
	public override void _Draw()
	{
		DrawArc(new Vector2(), 27.5f, 0, Mathf.Tau, 100, Colors.Beige, 5, true);
	}
}