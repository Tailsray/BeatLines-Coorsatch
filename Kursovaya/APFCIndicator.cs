using Godot;

public partial class APFCIndicator : Node2D
{
	int indicators = 2;

	public override void _Draw()
	{
		if (indicators == 2)
			DrawCircle(new Vector2(20, 20), 10, Colors.Yellow);
		if (indicators > 0)
			DrawCircle(new Vector2(-20, 20), 10, Colors.DeepSkyBlue);
	}

	public void SetIndicators(int count)
	{
		indicators = count;
		QueueRedraw();
	}
}
