using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class PlayerWeaponDataHolder : Node
{
	[Export]
	private Array<PlayerWeaponData> _playerWeaponData = new Array<PlayerWeaponData>();

	public List<PlayerWeaponData> Data => _playerWeaponData.ToList();
}
