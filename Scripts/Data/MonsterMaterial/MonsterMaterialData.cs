using Godot;

namespace MonsterHunterIdle;

public partial class MonsterMaterialData : MaterialData
{
	[Export] 
	private MonsterLevel _monsterLevel;
	
	public MonsterLevel MonsterLevel => _monsterLevel;
}
