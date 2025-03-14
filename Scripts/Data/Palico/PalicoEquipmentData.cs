using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public enum PalicoEquipmentType
{
	Weapon,
	Helmet,
	Armor
}

public partial class PalicoEquipmentData : Resource
{
	[Export]
	private string _name;

	[Export(PropertyHint.MultilineText)]
	private string _description;

	[Export]
	private Texture2D _texture;

	[Export]
	private PalicoEquipmentType _equipmentType;

	[Export]
	private Array<ResourceRequirementData> _resources;

	public string Name => _name;
	public string Description => _description;
	public Texture2D Texture => _texture;
	public PalicoEquipmentType EquipmentType => _equipmentType;
	public EquipmentLevel EquipmentLevel = new EquipmentLevel();
	public List<ResourceRequirementData> Resources => _resources.ToList();
	
	public bool HasCreated = false;
}
