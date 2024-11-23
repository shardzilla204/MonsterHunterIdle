using Godot;

namespace MonsterHunterIdle;

public partial class BiomeDisplay : Control
{
	[Export] 
	private Label _biomeLabel;

	[Export]
	private TextureRect _biomeIcon;

	[Export]
	private TextureRect _biomeBackground;

	private BiomeData _biomeData;
	
    public override void _Ready()
    {
		_biomeData = BiomeManager.Instance.Biome;
		BiomeManager.Instance.Updated += Update;

		Update();
    }

    private void Update()
	{
		_biomeData = BiomeManager.Instance.Biome;
		_biomeLabel.Text = _biomeData.Name;
		_biomeIcon.Texture = _biomeData.Icon;
		_biomeBackground.Texture = _biomeData.Background;
	}
}
