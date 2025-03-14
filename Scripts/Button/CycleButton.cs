using Godot;

namespace MonsterHunterIdle;

public partial class CycleButton : CustomButton
{
	[Export]
	private bool _isClockwise = false;

	[Export]
	private TextureRect _biomeIcon;

	private int _biomePosition;

	public override void _Ready()
	{	
		base._Ready();
		_biomePosition = _isClockwise ? 2 : 1;

		Pressed += () => BiomeManager.Instance.CycleBiome(_isClockwise);
		BiomeManager.Instance.Updated += SetIcon;

		SetIcon();
	}

	private void SetIcon()
	{
		BiomeType biomeType = BiomeManager.Instance.BiomesQueue[_biomePosition];
		BiomeData biomeData = BiomeManager.Instance.Biomes[biomeType];
		_biomeIcon.Texture = biomeData.Icon;
	}
}
