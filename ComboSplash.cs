using Godot;

public partial class ComboSplash : Label
{
	public string Grade { get; set; }
	public string Combo { get; set; }
	private string Timing { get; set; }
	int frames { get; set; }
	public Vector2 InitPos { get; set; }

	public override void _Ready()
	{
		Text = $"{Timing}{Grade}{Combo}";

		AddThemeConstantOverride("outline_size", 8);
		switch (Grade)
		{
			case "NICE":
				AddThemeColorOverride("font_color", Colors.SpringGreen);
				break;
			case "OFF":
				AddThemeColorOverride("font_color", Colors.BlueViolet);
				break;
			case "BAD":
				AddThemeColorOverride("font_color", Colors.PaleVioletRed);
				break;
		}
	}

	public override void _Process(double delta)
	{
		Position = InitPos - new Vector2(50, 100 * (1 - Mathf.Pow(1 - frames / 30f, 3)));
		Modulate = (Color.Color8((byte)GetThemeColor("font_color").R8,
								 (byte)GetThemeColor("font_color").G8,
								 (byte)GetThemeColor("font_color").B8,
								 (byte)(255 * (1 - Mathf.Clamp(frames - 10, 0, 20) / 20f))));

		if (frames > 30)
			QueueFree();

		frames++;
	}
}