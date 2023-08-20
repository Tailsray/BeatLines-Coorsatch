using Godot;

public partial class PauseMenu : PanelContainer
{
	Button resumeButton, retryButton, rebindButton, exitButton;

	public override void _Ready()
	{
		resumeButton = GetNode<Button>("ButtonBoxes/ResumeBox/ResumeButton");
		retryButton = GetNode<Button>("ButtonBoxes/RetryBox/RetryButton");
		rebindButton = GetNode<Button>("ButtonBoxes/RebindBox/RebindButton");
		exitButton = GetNode<Button>("ButtonBoxes/ExitBox/ExitButton");
	}

	public void OnPaused()
	{
		resumeButton.Disabled = false;
		retryButton.Disabled = false;
		rebindButton.Disabled = false;
		exitButton.Disabled = false;

		resumeButton.GrabFocus();
	}
}
