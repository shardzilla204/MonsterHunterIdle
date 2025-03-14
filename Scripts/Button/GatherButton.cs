using Godot;

namespace MonsterHunterIdle;

public partial class GatherButton : CustomButton
{
	[Export]
	private TextureProgressBar _gatherProgress;

	[Export]
	private TextureRect _gatherIcon;

	private bool _isDown = false;
	private const float _ProgressValue = 0.5f;

	public override void _Ready()
	{
		base._Ready();
		ButtonDown += () => _isDown = true;
		ButtonUp += () => _isDown = false;
		BiomeManager.Instance.Updated += SetIcon;
		TreeExited += () => BiomeManager.Instance.Updated -= SetIcon; // Removes the signal when being removed
		
		SetIcon();
	}

	public override void _Process(double delta)
	{
		CheckProgress();
	}

	private void CheckProgress()
	{
		_gatherProgress.Value += _isDown ? _ProgressValue : -_ProgressValue;

		if (_gatherProgress.Value < _gatherProgress.MaxValue) return;

		_gatherProgress.Value = 0;

		GD.PrintRich($"Picked Up {PrintRich.SetTextColor(BiomeManager.Instance.AddMaterial().Name, TextColor.Yellow)}");
		MonsterManager.Instance.Encounter.Check();
	}

	private void SetIcon()
	{
		_gatherIcon.Texture = BiomeManager.Instance.Biome.GatherIcon;
	}
}
