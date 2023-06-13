using Godot;
using System.Collections.Generic;

public partial class ScoreTable : Label
{
	List<int> Scores { get; set; }
	int CurrentScore { get; set; }
	int MaxScore { get; set; }
	double finalScore { get; set; }
	int prevScore { get; set; }
	double lastNoteTiming { get; set; }
	MusicSource MS;
	public void InitReferences(MusicSource _ms)
	{
		MS = _ms;
	}
	public void OnNoteHit(int kek, float rofl) 
	{
		if(kek != 3)
		{
			prevScore = CurrentScore;
			
			lastNoteTiming = (double)GetTree().GetNodesInGroup("notes")[0].Call("GetTiming");
			
		}
	}
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
		
		if(1 - Mathf.Pow(1 - (MS.CurrentTime - lastNoteTiming), 3) <= 1)
		{
		finalScore = prevScore + ((1 - Mathf.Pow(1 - (MS.CurrentTime - lastNoteTiming), 3))) * (CurrentScore - prevScore);
		}
		
		
	}

	public int Score()
	{
		return Mathf.FloorToInt(finalScore / MaxScore * 1000000);
	}

	public override void _Process(double delta)
	{
		Scores.RemoveAt(0);
		Scores.Add(CurrentScore);
		Text = Score().ToString();
		
	}
}
