using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class PalicoArmorDataHolder : Node
{
	[Export]
	private Array<PalicoArmorData> _palicoArmorData = new Array<PalicoArmorData>();

	public List<PalicoArmorData> Data => _palicoArmorData.ToList();
}
