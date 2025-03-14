using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public enum PlayerEquipmentType
{
	Weapon,
	Helmet,
	Chestmail,
	Vambraces,
	Belt,
	Greaves
}

public partial class PlayerEquipmentData : Resource
{
	[Export]
	private string _name;

	[Export(PropertyHint.MultilineText)]
	private string _description;

	[Export]
	private Texture2D _texture;

	[Export]
	private PlayerEquipmentType _equipmentType;

	[Export]
	private Array<ResourceRequirementData> _resources;

	public string Name => _name;
	public string Description => _description;
	public Texture2D Texture => _texture;
	public PlayerEquipmentType EquipmentType => _equipmentType;
	public EquipmentLevel EquipmentLevel = new EquipmentLevel();
	public List<ResourceRequirementData> Resources => _resources.ToList();

	public bool HasCreated = false;
}

public class EquipmentLevel
{
	public int Level = 0;
	public const int MaxLevel = 10;
	public int Current = 0;
	public int Maxmimum = 100;
}
