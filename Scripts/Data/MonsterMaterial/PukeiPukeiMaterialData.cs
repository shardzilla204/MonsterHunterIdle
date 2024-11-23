using Godot;

namespace MonsterHunterIdle;

public enum PukeiPukeiMaterial
{
	Scale,
	Shell,
	Tail,
	Sac,
	Quill,
	Primescale,
	WyvernGemShard
}

[GlobalClass]
public partial class PukeiPukeiMaterialData : MonsterMaterialData
{
	[Export]
	public PukeiPukeiMaterial Material;
}
