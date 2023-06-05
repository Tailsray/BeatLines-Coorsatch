using Godot;

public partial class MusicSource : AudioStreamPlayer
{
	public float BPM;
	public double CurrentTime;

    public override void _Ready()
    {
        CurrentTime = 0;
    }

    public override void _Process(double delta)
    {
		CurrentTime = (double)((GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency()) / 60f * BPM);
        GetTree().CallGroup("paths", "UpdateTime", CurrentTime);
    }
}