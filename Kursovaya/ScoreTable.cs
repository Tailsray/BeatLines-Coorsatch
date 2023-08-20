using Godot;

public partial class ScoreTable : Label
{
	int CurrentScore { get; set; }
	int MaxScore { get; set; }
	double FinalScore { get; set; }
	int PrevScore { get; set; }
	double LastNoteTiming { get; set; }

	public void OnNoteHit(bool success, int dscore, double t, float pos) 
	{
		if (success)
		{
			PrevScore = CurrentScore;
			LastNoteTiming = t;
		}
	}
	public override void _Ready()
	{
		CurrentScore = 0;
		MaxScore = 1;
	}

	public void SetMaxScore(int score)
	{
		MaxScore = score;
	}

	public void UpdateScore(double time, int score)
	{
		CurrentScore = score;

		double t = 1 - Mathf.Pow(1 - (time - LastNoteTiming), 3);
		FinalScore = PrevScore + (CurrentScore - PrevScore) * Mathf.Clamp(t, 0, 1);
	}

	public int Score()
	{
		return Mathf.FloorToInt(FinalScore / MaxScore * 1000000);
	}

	public override void _Process(double delta)
	{
		Text = Score().ToString();
	}
}