using Godot;

namespace MonsterHunterIdle;

public enum ForestMaterial
{
	// * Herbs
	FireHerb,
	Flowfern,
	SnowHerb,
	SleepHerb,

	// * Shrooms
	Parashroom,
	Toadstool,
	
	// * Bugs
	Thunderbug,
	Godbug,
	Carpenterbug,
	// * Berries
	DragonfellBerry,
}

[GlobalClass]
public partial class ForestMaterialData : MaterialData
{
	[Export]
	private ForestMaterial _material;

	public ForestMaterial Material => _material;
}
