using Godot;

public enum ArmorSphereType
{
	ArmorSphere,
	ArmorSpherePlus,
	AdvancedArmorSphere,
	HardArmorSphere,
	HeavyArmorSphere,
	RoyalArmorSphere,
	TrueArmorSphere
}

[GlobalClass]
public partial class ArmorSphereData : Resource
{
	[Export]
	private ArmorSphereType _armorSphereType;

	[Export]
	private int _amount;

	public ArmorSphereType ArmorSphereType => _armorSphereType;
	public int Amount => _amount;
}
