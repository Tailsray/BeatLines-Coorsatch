using Godot;

public partial class Note : Sprite2D
{
	[Signal]
	public delegate void NoteHitEventHandler(bool missed, int dscore, double t, float pos);

	int Path1ID { get; set; }
	int Path2ID { get; set; }
	double MyTime { get; set; }
	int TapsToGo { get; set; }
	int Grade { get; set; }
	double timing { get; set; }
	MusicSource MS;
	StateMachine SM;

	public void InitNote(StateMachine _sm, MusicSource _ms,
						 int path1ID, int path2ID,
						 double myTime, int tapsToGo)
	{
		SM = _sm;
		MS = _ms;
		Path1ID = path1ID;
		Path2ID = path2ID;
		MyTime = myTime;
		TapsToGo = tapsToGo;
	}

	public void Pressed()
	{
		if (Mathf.Abs(timing) <= 0.13)
			Hit(true);
	}

	void Hit(bool success)
	{
		if (--TapsToGo >= 0)
			EmitSignal(SignalName.NoteHit,
							success,
							GetGrade(),
							MS.CurrentTime,
							SM.GetX((TapsToGo == 0 ? Path1ID : Path2ID), MyTime));
		if (TapsToGo == 0)
			QueueFree();
	}

	int GetGrade()
	{
		return (Mathf.Abs(timing) <= 0.05 ? 1 : 0)
			 + (Mathf.Abs(timing) <= 0.09 ? 1 : 0);
	}

	public override void _Ready()
	{
		AddToGroup("notes");
	}

	public override void _Process(double delta)
	{
		timing = (MS.CurrentTime - MyTime) * 60 / MS.BPM;

		if (timing > 0.13)
		{
			if (TapsToGo == 2)
				Hit(false);
			Hit(false);
		}

		Position = new Vector2(SM.GetX(Path1ID, MyTime),
							   SM.GetY(MyTime - MS.CurrentTime));
	}

	public override void _Draw()
	{
		if (Path2ID == 0)
		{
			DrawCircle(new Vector2(), 25, Colors.Black);
			DrawCircle(new Vector2(), 15, Colors.SpringGreen);
		}
		else
		{
			var x2 = SM.GetX(Path2ID, MyTime) - SM.GetX(Path1ID, MyTime);

			DrawCircle(new Vector2(), 25, Colors.Gold);
			DrawCircle(new Vector2(), 22, Colors.Black);
			DrawCircle(new Vector2(), 15, Colors.SpringGreen);

			DrawCircle(new Vector2(x2, 0), 25, Colors.Gold);
			DrawCircle(new Vector2(x2, 0), 22, Colors.Black);
			DrawCircle(new Vector2(x2, 0), 15, Colors.SpringGreen);
		}
	}
}
