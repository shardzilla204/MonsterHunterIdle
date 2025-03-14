using Godot;

namespace MonsterHunterIdle;

public partial class BiomeInterface : Control
{
	[Export] 
	private Label _biomeLabel;

	[Export]
	private TextureRect _biomeLocaleIcon;

	[Export]
	private TextureRect _biomeBackground;
	
    public override void _Ready()
    {
		Biome biome = MonsterHunterIdle.BiomeManager.Biome;
		_biomeLabel.Text = biome.Name;
		_biomeLocaleIcon.Texture = biome.LocaleIcon;
		_biomeBackground.Texture = biome.Background;
    }
}
