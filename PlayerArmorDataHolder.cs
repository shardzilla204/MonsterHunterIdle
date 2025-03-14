using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class PlayerArmorDataHolder : Node
{
	[Export]
	private Array<PlayerArmorData> _playerArmorData = new Array<PlayerArmorData>();

	public List<PlayerArmorData> Data => _playerArmorData.ToList();
}
