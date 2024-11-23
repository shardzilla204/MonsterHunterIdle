using Godot;

namespace MonsterHunterIdle;

public enum SwampMaterial
{
	// * Ores
	IronOre,
	MachaliteOre,
	DragoniteOre,
	EarthCrystal
}

[GlobalClass]
public partial class SwampMaterialData : MaterialData
{
	[Export]
	private SwampMaterial _material;
	
	public SwampMaterial Material => _material;
}