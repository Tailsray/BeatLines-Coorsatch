using System.Collections.Generic;
using Godot;

public partial class Path : Node2D
{
	Follower follower;
	public int MyID { get; set; }
	bool FollowerVisible { get; set; } = true;
	int currentState = -1;
	double CurrentTime { get; set; }
	StateMachine SM;
	List<StateMachine.State> states;
	Vector2[] points;

	public void InitStates(StateMachine _sm)
	{
		SM = _sm;
		states = SM.GetStates(MyID);
	}

	void UpdateTime(double time)
	{
		CurrentTime = time;
	}

	public override void _Ready()
	{
		AddToGroup("paths");

		follower = ResourceLoader.Load<PackedScene>("res://Follower.tscn").Instantiate<Follower>();
		AddChild(follower);

		points = new Vector2[31];
	}

	public override void _Process(double delta)
	{
		if (currentState + 1 < states.Count && states[currentState + 1].t < CurrentTime)
			currentState++;

		if (currentState < 0 || states[currentState].type == StateMachine.CurveType.End)
			follower.Hide();
		else
			follower.Show();
		follower.Position = new Vector2(SM.GetX(MyID, CurrentTime), 600f);

		QueueRedraw();
	}

	public override void _Draw()
	{
		for (var i = (Mathf.Clamp(currentState - 5,  0, states.Count - 1));
			 i < (Mathf.Clamp(currentState + 20, 0, states.Count - 1));
			 i++)
			if (states[i].type != StateMachine.CurveType.End)
				DrawChartLine(states[i].t, states[i + 1].t);
	}

	void DrawChartLine(double t1, double t2)
	{
		for (var i = 0; i < points.Length; i++)
		{
			var ti = t1 + i / 30.0 * (t2 - t1);
			points[i] = new Vector2(SM.GetX(MyID, ti), SM.GetY(ti - CurrentTime));
		}
		DrawPolyline(points, Colors.Red, 5, true);
	}
}
