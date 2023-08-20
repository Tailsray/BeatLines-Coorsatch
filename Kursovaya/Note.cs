using Godot;

public partial class Note : Sprite2D
{
	[Signal]
	public delegate void NoteHitEventHandler(int deltascore, float pos);

	public int Path1ID { get; set; }
	public int Path2ID { get; set; }
	public double MyTime { get; set; }
	public int TapsToGo { get; set; }
	int Grade { get; set; }
	double timing { get; set; }
	MusicSource MS;
	StateMachine SM;

	public void InitReferences(StateMachine _sm, MusicSource _ms)
	{
		SM = _sm;
		MS = _ms;
	}

	public void Pressed()
	{
		if (Mathf.Abs(timing) <= 0.13)
		{
			Grade = Grade < GetGrade() ? Grade : GetGrade();
			Hit(Grade);
		}
	}

	void Hit(int grade)
	{
		if (--TapsToGo >= 0)
			EmitSignal("NoteHit", grade, SM.getX((TapsToGo == 0 ? Path1ID : Path2ID), MyTime));
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
		Grade = 3;
	}

    public override void _Process(double delta)
    {
		timing = (MS.CurrentTime - MyTime) * 60 / MS.BPM;

		if (timing > 0.13)
		{
			if (TapsToGo == 2)
				Hit(3);
			Hit(3);
		}

        Position = new Vector2(SM.getX(Path1ID, MyTime),
							   SM.getY(MyTime - MS.CurrentTime));
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
			var x2 = SM.getX(Path2ID, MyTime) - SM.getX(Path1ID, MyTime);

			DrawCircle(new Vector2(), 25, Colors.Gold);
			DrawCircle(new Vector2(), 22, Colors.Black);
			DrawCircle(new Vector2(), 15, Colors.SpringGreen);

			DrawCircle(new Vector2(x2, 0), 25, Colors.Gold);
			DrawCircle(new Vector2(x2, 0), 22, Colors.Black);
			DrawCircle(new Vector2(x2, 0), 15, Colors.SpringGreen);
		}
    }
}