using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class SwampData : BiomeData
{
	[Export]
	public Array<SwampMaterialData> Materials = new Array<SwampMaterialData>();
}
