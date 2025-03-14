using Godot;

namespace MonsterHunterIdle;

public partial class Biome : Node
{
	public Biome(BiomeType biomeType)
	{
		Name = biomeType.ToString();
		Type = biomeType;
		LocaleIcon = MonsterHunterIdle.BiomeManager.GetLocaleIcon(biomeType);
		Background = MonsterHunterIdle.BiomeManager.GetBackground(biomeType);
		GatherIcon = MonsterHunterIdle.BiomeManager.GetGatherIcon(biomeType);
	}

	public new string Name;
	public BiomeType Type;
	public Texture2D LocaleIcon;
	public Texture2D Background;
	public Texture2D GatherIcon;
}