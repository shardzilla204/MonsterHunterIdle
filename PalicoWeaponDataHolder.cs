using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class PalicoWeaponDataHolder : Node
{
	[Export]
	private Array<PalicoWeaponData> _palicoWeaponData = new Array<PalicoWeaponData>();

	public List<PalicoWeaponData> Data => _palicoWeaponData.ToList();
}
