using Godot;

namespace MonsterHunterIdle;

public enum DesertMaterial
{
	// * Bones
	MonsterBoneSmall = 600,
	MonsterBoneMedium = 500,
	MonsterBoneLarge = 400,
	MonsterBonePlus = 400,
}

[GlobalClass]
public partial class DesertMaterialData : MaterialData
{
	[Export]
	private DesertMaterial _material;
	
	public DesertMaterial Material => _material;
}