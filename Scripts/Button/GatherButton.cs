using Godot;

namespace MonsterHunterIdle;

public partial class GatherButton : Button
{
	[Signal]
	public delegate void FinishedEventHandler();

	[Export]
	private TextureProgressBar _gatherProgress;

	[Export]
	private TextureRect _gatherIcon;

	private bool _isDown = false;

	public override void _Ready()
	{
		ButtonDown += () => _isDown = true;
		ButtonUp += () => _isDown = false;
		BiomeManager.Instance.Updated += SetIcon;
		TreeExited += () => { BiomeManager.Instance.Updated -= SetIcon; }; // Removes the signal when being removed
		
		SetIcon();
	}

	public override void _Process(double delta)
	{
		CheckProgress();
	}

	private void CheckProgress()
	{
		const float progressValue = 0.1f;
		_gatherProgress.Value += _isDown ? progressValue : -progressValue;

		if (_gatherProgress.Value < _gatherProgress.MaxValue) return;

		_gatherProgress.Value = 0;

		EmitSignal(SignalName.Finished);
		MonsterManager.Instance.CheckEncounter();
	}

	private void SetIcon()
	{
		_gatherIcon.Texture = BiomeManager.Instance.Biome.GatherIcon;
	}
}
