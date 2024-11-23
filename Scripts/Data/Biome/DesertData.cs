using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class DesertData : BiomeData
{
	[Export]
	public Array<DesertMaterialData> Materials = new Array<DesertMaterialData>();
}
