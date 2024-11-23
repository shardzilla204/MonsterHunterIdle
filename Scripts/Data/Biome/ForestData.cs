using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class ForestData : BiomeData
{
	[Export]
	public Array<ForestMaterialData> Materials = new Array<ForestMaterialData>();
}
