using Godot;

public partial class RebindMenu : PanelContainer
{
	string helpText = "Left/Right: Choose\nEnter: Edit\nEsc: Save and exit";
	string waitText = "Waiting for input...";

	int Awaited { get; set; }

	Label HelpLabel;
	Godot.Collections.Array<Button> KeyButtons;

	public override void _Ready()
	{
		KeyButtons = new Godot.Collections.Array<Button>();

		HelpLabel = GetNode<Label>("UI/HelpText");
		for (int i = 1; i <= 4; i++)
		{	
			KeyButtons.Add(GetNode<Button>($"UI/KeyButtons/Key{i}ButtonMargin/Key{i}Button"));
			if (InputMap.ActionGetEvents($"key{i}")[0] is InputEventKey a)
				KeyButtons[i - 1].Text = a.AsTextPhysicalKeycode();
		}

		HelpLabel.Text = helpText;
		Awaited = 0;
		KeyButtons[0].GrabFocus();
	}

    public void BindKey(int index)
	{
		HelpLabel.Text = waitText;
		for (int i = 0; i < 4; i++)
			if (i != index - 1)
				KeyButtons[i].Disabled = true;
		Awaited = index;
	}

    public override void _Input(InputEvent ev)
    {
		if (ev.IsActionPressed("pause"))
		{
			GetNode<Button>("../PauseMenu/ButtonBoxes/RebindBox/RebindButton").GrabFocus();
			QueueFree();
		}
        else if (Awaited != 0)
		{
			InputMap.ActionEraseEvents($"key{Awaited}");
			InputMap.ActionAddEvent($"key{Awaited}", ev);
			if (ev is InputEventKey a)
				KeyButtons[Awaited - 1].Text = a.AsTextKeycode();
			HelpLabel.Text = helpText;
			foreach (Button b in KeyButtons)
				b.Disabled = false;
			Awaited = 0;
		}
    }
}
