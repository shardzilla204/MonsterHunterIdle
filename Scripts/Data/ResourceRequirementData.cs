using Godot;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class ResourceRequirementData : Resource
{
	[Export]
	private MaterialData _material;
	
	[Export]
	private int _amount;

	public MaterialData Material => _material;
	public int Amount => _amount;
}
