using Godot;

public partial class ComboSplash : Label
{
	public string Grade { get; set; }
	public string Combo { get; set; }
	public string Timing { get; set; }
	int frames { get; set; }

	public override void _Ready()
	{
		Position += new Vector2(-50, 0);
		Text = $"{Timing}{Grade}{Combo}";

		AddThemeConstantOverride("outline_size", 8);
		if (Grade == "NICE")
			AddThemeColorOverride("font_color", Colors.SpringGreen);
		if (Grade == "OFF")
			AddThemeColorOverride("font_color", Colors.BlueViolet);
		if (Grade == "BAD")
			AddThemeColorOverride("font_color", Colors.PaleVioletRed);

	}

	public override void _Process(double delta)
	{
		if (frames < 10 || frames > 20)
			Position += new Vector2(0, -10);

		if (frames > 20)
		{
			Modulate = (Color.Color8((byte)GetThemeColor("font_color").R8,
									 (byte)GetThemeColor("font_color").G8,
									 (byte)GetThemeColor("font_color").B8,
									 (byte)(255 * (frames - 10) / 30)));
		}

		if (frames > 30)
			QueueFree();

		frames++;
	}
}
