using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class CraftingInterface : Container
{
	[Export]
	private Container _craftButtonContainer;

	[Export]
	private CustomButton _filterButton;

	private CraftButton _craftButton;
	
	private CraftingFilterInterface _craftingFilterInterface;

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
		_filterButton.Toggled += OnFilterButtonToggled;

		// Refresh the equipment
		OnEquipmentChanged();
	}

	private void OnFilterButtonToggled(bool isToggled)
	{
		if (isToggled)
		{
			_craftingFilterInterface = MonsterHunterIdle.PackedScenes.GetCraftingFilterInterface();
			_craftingFilterInterface.FiltersChanged += OnFiltersChanged;
			AddChild(_craftingFilterInterface);
		}
		else
		{
			_craftingFilterInterface.QueueFree();
		}
	}

	private void OnFiltersChanged(GC.Dictionary<string, bool> filters)
	{
		ClearEquipment();
		ShowFilteredEquipment(filters);
	}

	private void OnEquipmentChanged(Equipment equipment = null)
	{
		ClearEquipment();

		List<Equipment> weapons = [.. MonsterHunterIdle.EquipmentManager.Weapons];
		ShowEquipment(weapons);

		List<Equipment> armor = [.. MonsterHunterIdle.EquipmentManager.Armor];
		ShowEquipment(armor);

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.CraftButtonPressed, equipment);
	}

	private void OnEquipmentAdded(Equipment equipment = null)
	{
		ChangeEquipmentInterface changeEquipmentInterface = MonsterHunterIdle.PackedScenes.GetChangeEquipmentInterface(equipment);
		AddSibling(changeEquipmentInterface);
	}

	private void ShowFilteredEquipment(GC.Dictionary<string, bool> filters)
	{
		// Check if any filter is true, if all are false then just show all the equipment
		bool hasFilter = HasFilter(filters);
		if (hasFilter)
		{
			List<Equipment> filteredEquipment = new List<Equipment>();

			foreach (string filterName in filters.Keys)
			{
				bool isFiltered = filters[filterName];
				if (!isFiltered) continue;

				bool isSuccessful = Enum.TryParse(filterName, out WeaponCategory weaponCategory);
				if (isSuccessful)
				{
					List<Weapon> weapons = [.. MonsterHunterIdle.EquipmentManager.Weapons];
					List<Weapon> categoryWeapons = weapons.FindAll(weapon => weapon.Category == weaponCategory);

					filteredEquipment.AddRange(categoryWeapons);
					continue;
				}

				isSuccessful = Enum.TryParse(filterName, out ArmorCategory armorCategory);
				if (isSuccessful)
				{
					List<Armor> armor = [.. MonsterHunterIdle.EquipmentManager.Armor];
					List<Armor> categoryArmor = armor.FindAll(armor => armor.Category == armorCategory);

					filteredEquipment.AddRange(categoryArmor);
					continue;
				}

				if (filterName.Contains("HasNotCrafted"))
				{
					filteredEquipment = [.. filteredEquipment.FindAll(equipment => !MonsterHunterIdle.EquipmentManager.HasCrafted(equipment))];
				}
			}

			ShowEquipment(filteredEquipment);
		}
		else
		{
			List<Equipment> weapons = [.. MonsterHunterIdle.EquipmentManager.Weapons];
			ShowEquipment(weapons);

			List<Equipment> armor = [.. MonsterHunterIdle.EquipmentManager.Armor];
			ShowEquipment(armor);
		}
	}

	private bool HasFilter(GC.Dictionary<string, bool> filters)
	{
		bool hasFilter = filters.Values.ToList().Contains(true);
		return hasFilter;
	}

	private void ShowEquipment(List<Equipment> equipment)
	{
		foreach (Equipment equipmentPiece in equipment)
		{
			CraftButton craftButton = MonsterHunterIdle.PackedScenes.GetCraftButton(equipmentPiece);
			craftButton.Pressed += () => SetEquipmentButton(craftButton);
			_craftButtonContainer.AddChild(craftButton);
		}
	}

	private void ClearEquipment()
	{
		GC.Array<Node> children = _craftButtonContainer.GetChildren();
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
