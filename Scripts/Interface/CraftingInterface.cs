using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
		// Show all the equipment if there are no filters
		bool hasFilter = HasFilter(filters);
		if (!hasFilter)
		{
			List<Equipment> weapons = [.. MonsterHunterIdle.EquipmentManager.Weapons];
			ShowEquipment(weapons);

			List<Equipment> armor = [.. MonsterHunterIdle.EquipmentManager.Armor];
			ShowEquipment(armor);
			return;
		}

		List<Equipment> filteredEquipment = new List<Equipment>();
		List<Equipment> filteredGroupEquipment = new List<Equipment>();
		foreach (string filterKey in filters.Keys)
		{
			List<Equipment> equipment = [.. MonsterHunterIdle.EquipmentManager.Weapons];
			equipment.AddRange([.. MonsterHunterIdle.EquipmentManager.Armor]);

			bool isFiltered = filters[filterKey];
			if (!isFiltered) continue;

			// Add weapon
			bool isWeaponFilter = Enum.TryParse(filterKey, out WeaponCategory weaponCategory);
			if (isWeaponFilter)
			{
				List<Equipment> availableWeapons = equipment.FindAll(piece => piece is Weapon);
				List<Weapon> weapons = new List<Weapon>();
				foreach (Equipment availableWeapon in availableWeapons)
				{
					Weapon weapon = availableWeapon as Weapon;
					weapons.Add(weapon);
				}
				List<Weapon> categoryWeapons = weapons.FindAll(weapon => weapon.Category == weaponCategory);

				filteredEquipment.AddRange(categoryWeapons);
				continue;
			}

			// Add armor
			bool isArmorFilter = Enum.TryParse(filterKey, out ArmorCategory armorCategory);
			if (isArmorFilter)
			{
				List<Equipment> availableArmor = equipment.FindAll(piece => piece is Armor);
				List<Armor> armor = new List<Armor>();
				foreach (Equipment availableArmorPiece in availableArmor)
				{
					Armor armorPiece = availableArmorPiece as Armor;
					armor.Add(armorPiece);
				}
				List<Armor> categoryArmor = armor.FindAll(armor => armor.Category == armorCategory);

				filteredEquipment.AddRange(categoryArmor);
				continue;
			}

			// Subtract to group equipment
			// Find all the equipment that matches the specified group
			bool isGroupFilter = Enum.TryParse(filterKey, out GroupCategory groupCategory);
			if (isGroupFilter)
			{
				// Filter through the current equipment if there is nothing filter through all the equipment
				if (filteredEquipment.Count > 0)
				{
					// * Current filtered equipment
					List<Equipment> groupEquipment = GetGroupEquipment(filteredEquipment, groupCategory);
					filteredGroupEquipment.AddRange(groupEquipment);
					continue;
				}
				else
				{
					// * All equipment
					List<Equipment> groupEquipment = GetGroupEquipment(equipment, groupCategory);
					filteredGroupEquipment.AddRange(groupEquipment);
					continue;
				}
			}

			// Subtract miscellaneous filters
			if (filterKey.Contains("HasNotCrafted"))
			{
				// Filter any equipment that has been filtered by group (tree/set)
				if (filteredGroupEquipment.Count > 0)
				{
					filteredGroupEquipment = [.. filteredGroupEquipment.FindAll(equipment => !MonsterHunterIdle.EquipmentManager.HasCrafted(equipment))];
					continue;
				}
				if (filteredEquipment.Count > 0)
				{
					filteredEquipment = [.. filteredEquipment.FindAll(equipment => !MonsterHunterIdle.EquipmentManager.HasCrafted(equipment))];
				}
				else
				{
					filteredEquipment = [.. equipment.FindAll(equipPiece => !MonsterHunterIdle.EquipmentManager.HasCrafted(equipPiece))];
				}
			}
		}

		if (filteredGroupEquipment.Count > 0)
		{
			ShowEquipment(filteredGroupEquipment);
		}
		else
		{
			ShowEquipment(filteredEquipment);
		}
	}

	private List<Equipment> GetGroupEquipment(List<Equipment> equipment, GroupCategory groupCategory)
	{
		string groupString = groupCategory.ToString();
		List<Equipment> groupEquipment = new List<Equipment>();
		foreach (Equipment equipmentPiece in equipment)
		{
			if (equipmentPiece is Weapon weapon)
			{
				WeaponTree targetTree;
				bool isSuccessful = Enum.TryParse(groupString, out targetTree);
				if (!isSuccessful || weapon.Tree != targetTree) continue;

				groupEquipment.Add(weapon);
			}
			else if (equipmentPiece is Armor armor)
			{
				string[] groupSubStrings = MonsterHunterIdle.AddSpacing(groupString).Split(" ");
				foreach (string groupSubString in groupSubStrings)
				{
					ArmorSet targetSet;
					bool isSuccessful = Enum.TryParse(groupSubString, out targetSet);
					if (!isSuccessful || armor.Set != targetSet) continue;

					groupEquipment.Add(armor);
				}
			}
		}
		return groupEquipment;
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
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Find {targetEquipment.Name}";
            PrintRich.PrintError(className, message);

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
