using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class ItemBoxData : Resource
{
	public List<dynamic> Materials = new List<dynamic>();
}
