using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

// TODO: Add filter buttons
public partial class CraftingInterface : NinePatchRect
{
	[Export]
	private Container _craftButtonContainer;

	private EntityType _entityType = EntityType.Player;
	private EquipmentType _equipmentType = EquipmentType.Weapon;

	private CraftButton _craftButton;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.EquipmentUpgraded -= OnEquipmentUpgraded;
		MonsterHunterIdle.Signals.EquipmentChanged -= OnEquipmentChanged;
		MonsterHunterIdle.Signals.WeaponAdded -= OnEquipmentAdded;
		MonsterHunterIdle.Signals.ArmorAdded -= OnEquipmentAdded;
    }

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.EquipmentUpgraded += OnEquipmentUpgraded;
		MonsterHunterIdle.Signals.EquipmentChanged += OnEquipmentChanged;
		MonsterHunterIdle.Signals.WeaponAdded += OnEquipmentAdded;
		MonsterHunterIdle.Signals.ArmorAdded += OnEquipmentAdded;
	}

	public override void _Ready()
	{
		// Refresh the equipment
		OnEquipmentChanged();
	}

	private void OnEquipmentChanged(Equipment equipment = null)
	{
		ClearEquipment();
		ShowEquipment();

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.CraftButtonPressed, equipment);
	}

	private void OnEquipmentAdded(Equipment equipment = null)
	{
		ChangeEquipmentInterface changeEquipmentInterface = MonsterHunterIdle.PackedScenes.GetChangeEquipmentInterface(equipment);
		AddSibling(changeEquipmentInterface);
	}

	private void ShowEquipment()
	{
		foreach (Weapon weapon in MonsterHunterIdle.EquipmentManager.Weapons)
		{
			CraftButton weaponCraftButton = MonsterHunterIdle.PackedScenes.GetCraftButton(weapon);
			weaponCraftButton.Pressed += () => SetEquipmentButton(weaponCraftButton);
			_craftButtonContainer.AddChild(weaponCraftButton);
		}

		foreach (Armor armor in MonsterHunterIdle.EquipmentManager.Armor)
		{
			CraftButton armorCraftButton = MonsterHunterIdle.PackedScenes.GetCraftButton(armor);
			armorCraftButton.Pressed += () => SetEquipmentButton(armorCraftButton);
			_craftButtonContainer.AddChild(armorCraftButton);
		}
	}

	private void ClearEquipment()
	{
		Array<Node> children = _craftButtonContainer.GetChildren();
		foreach (Node child in children)
		{
			_craftButtonContainer.RemoveChild(child);
			child.QueueFree();
		}
	}

	private void SetEquipmentButton(CraftButton equipmentCraftButton)
	{
		_craftButton = equipmentCraftButton;
	}

	private void OnEquipmentUpgraded(Equipment equipment)
	{
		Equipment targetEquipment = _craftButton.Equipment;
		Equipment hunterEquipment;

		if (targetEquipment is Weapon weapon)
		{
			hunterEquipment = MonsterHunterIdle.HunterManager.FindWeapon(weapon);
		}
		else if (targetEquipment is Armor armor)
		{
			hunterEquipment = MonsterHunterIdle.HunterManager.FindArmor(armor);
		}
		else
		{
			string errorMessage = $"Couldn't Find {targetEquipment.Name}";
			GD.PrintErr(errorMessage);

			return;
		}

		string previousEquipmentName = hunterEquipment.Name;
		int previousEquipmentSubGrade = hunterEquipment.SubGrade;

		MonsterHunterIdle.EquipmentManager.UpgradeEquipment(hunterEquipment);

		// Console message
		string previousSubGrade = previousEquipmentSubGrade == 0 ? "" : $" (+{previousEquipmentSubGrade})";
		string subGrade = hunterEquipment.SubGrade == 0 ? "" : $" (+{hunterEquipment.SubGrade})";
		string upgradedMessage = $"{previousEquipmentName}{previousSubGrade} Has Been Successfully Upgraded To {hunterEquipment.Name}{subGrade}";
		PrintRich.PrintLine(TextColor.Orange, upgradedMessage);

		OnEquipmentChanged(equipment);
	}
}
