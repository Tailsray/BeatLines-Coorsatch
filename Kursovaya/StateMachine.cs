using Godot;
using System.Collections.Generic;

public partial class StateMachine : Node
{
	public List<List<State>> states;
	public float Speed { get; set; }
    const float beatLength = 150f;

	public override void _Ready()
	{
		states = new List<List<State>>();
		Speed = 1;
	}

	public override void _Process(double delta)
	{
		
	}

	public enum CurveType
	{
		Straight,
		CircleDecel,
		CircleAccel,
		End,
		None
	}

	static CurveType Parse(string s)
	{
		if (s == "S")
			return StateMachine.CurveType.Straight;
		if (s == "A")
			return StateMachine.CurveType.CircleAccel;
		if (s == "D")
			return StateMachine.CurveType.CircleDecel;
		if (s == "E")
			return StateMachine.CurveType.End;

		return StateMachine.CurveType.None;
	}

	public partial class State : GodotObject
	{
		public State(double _t, float _pos, string _type)
		{
			t = _t;
			pos = _pos;
			type = Parse(_type);
		}

		public double t;
		public float pos;
		public CurveType type;
	}

	public void AddNewIndex()
	{
		states.Add(new List<State>());
	}

	public void AddState(int index, State state)
	{
		states[index - 1].Add(state);
	}

	public List<State> GetStates(int id)
	{
		return states[id - 1];
	}

	public float getX(int index, double time)
	{
		var s = states[index - 1];

		if (!(time >= s[0].t) || !(time <= s[^1].t)) return -1000;
		int i = 0;
		while (time > s[i + 1].t) i++;
		if (s[i].type == CurveType.End) i++;

		var a = (float)(time - s[i].t) / (float)(s[i + 1].t - s[i].t);
		return s[i].type switch
		{
			CurveType.Straight => s[i].pos + (s[i + 1].pos - s[i].pos) * a,
			CurveType.CircleDecel => s[i].pos + (s[i + 1].pos - s[i].pos) * Mathf.Sin(Mathf.Pi / 2 * a),
			CurveType.CircleAccel => s[i].pos + (s[i + 1].pos - s[i].pos) * (1 - Mathf.Cos(Mathf.Pi / 2 * a)),
			_ => -1000
		};
	}

	public float getY(double t)
	{
		return 600f - Speed * beatLength * (float)t;
	}
}