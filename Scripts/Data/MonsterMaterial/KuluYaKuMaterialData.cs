using Godot;

namespace MonsterHunterIdle;

public enum KuluYaKuMaterial
{
	Scale,
	Hide,
	Beak,
	Plume,
	Primescale,
	Primehide,
	WyvernGemShard
}

[GlobalClass]
public partial class KuluYaKuMaterialData : MonsterMaterialData
{
	[Export]
	public KuluYaKuMaterial Material;
}
