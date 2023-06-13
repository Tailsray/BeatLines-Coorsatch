using Godot;
using System.Collections.Generic;

public partial class ScoreTable : Label
{
	List<int> Scores { get; set; }
	int CurrentScore { get; set; }
	int MaxScore { get; set; }

	public override void _Ready()
	{
		Scores = new List<int> {0, 0, 0, 0, 0, 0, 0, 0};
		CurrentScore = 0;
		MaxScore = 1;
	}

	public void SetMaxScore(int score)
	{
		MaxScore = score;
	}

	public void UpdateScore(int score)
	{
		CurrentScore = score;
	}

	public double Score()
	{
		return Mathf.FloorToInt((Scores[0] * 0.05 +
								 Scores[1] * 0.10 +
								 Scores[2] * 0.20 +
								 Scores[3] * 0.35 +
								 Scores[4] * 0.55 +
								 Scores[5] * 0.90 +
								 Scores[6] * 2.00 +
								 Scores[7] * 3.85) / MaxScore * 125000);
	}

	public override void _Process(double delta)
	{
		Scores.RemoveAt(0);
		Scores.Add(CurrentScore);

		Text = Score().ToString();
	}
}
