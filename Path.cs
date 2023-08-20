using System.Collections.Generic;
using Godot;

public partial class Path : Node2D
{
	Follower follower;
	public int MyID { get; set; }
	bool FollowerVisible { get; set; } = true;
	int currentState = -1;
	const int vertexCount = 20;
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

		points = new Vector2[vertexCount + 1];

		QueueRedraw();
	}

	public override void _Process(double delta)
	{
		if (currentState + 1 < states.Count && states[currentState + 1].t < CurrentTime)
			currentState++;

		if (currentState < 0 || states[currentState].type == StateMachine.CurveType.End)
			follower.Hide();
		else
			follower.Show();

		Position = new Vector2(0, 600f - SM.GetY(CurrentTime));
		follower.GlobalPosition = new Vector2(SM.GetX(MyID, CurrentTime), 600f);
	}

	public override void _Draw()
	{
		for (var i = 0; i < states.Count - 1; i++)
			if (states[i].type != StateMachine.CurveType.End)
			{
				for (var j = 0; j <= vertexCount; j++)
				{
					var t = states[i].t + (states[i + 1].t - states[i].t) / vertexCount * j;
					points[j] = new Vector2(SM.GetX(MyID, t), SM.GetY(t));
				}
				DrawPolyline(points, Colors.Red, 5, true);
			}
	}
}
