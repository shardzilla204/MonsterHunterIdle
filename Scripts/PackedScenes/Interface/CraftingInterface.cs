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

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.FiltersChanged -= OnFiltersChanged;
		MonsterHunterIdle.Signals.EquipmentAdded -= OnEquipmentAdded;
		MonsterHunterIdle.Signals.EquipmentUpgraded -= OnEquipmentUpgraded;
		MonsterHunterIdle.Signals.PalicoEquipmentAdded -= OnPalicoEquipmentAdded;
		MonsterHunterIdle.Signals.PalicoEquipmentUpgraded -= OnPalicoEquipmentUpgraded;
		MonsterHunterIdle.Signals.CraftButtonPressed -= OnCraftButtonPressed;
		MonsterHunterIdle.Signals.PalicoCraftButtonPressed -= OnPalicoCraftButtonPressed;
		MonsterHunterIdle.Signals.PalicoCraftOptionButtonPressed -= OnPalicoCraftOptionButtonPressed;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.FiltersChanged += OnFiltersChanged;
		MonsterHunterIdle.Signals.EquipmentAdded += OnEquipmentAdded;
		MonsterHunterIdle.Signals.EquipmentUpgraded += OnEquipmentUpgraded;
		MonsterHunterIdle.Signals.PalicoEquipmentAdded += OnPalicoEquipmentAdded;
		MonsterHunterIdle.Signals.PalicoEquipmentUpgraded += OnPalicoEquipmentUpgraded;
		MonsterHunterIdle.Signals.CraftButtonPressed += OnCraftButtonPressed;
		MonsterHunterIdle.Signals.PalicoCraftButtonPressed += OnPalicoCraftButtonPressed;
		MonsterHunterIdle.Signals.PalicoCraftOptionButtonPressed += OnPalicoCraftOptionButtonPressed;
	}

	public override void _Ready()
	{
		_filterButton.Toggled += (isToggled) => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.FilterButtonToggled, isToggled);

		// Refresh the equipment
		RefreshEquipment();
	}

	// * START - Signal Functions
	private void OnFiltersChanged(GC.Dictionary<string, bool> filters)
	{
		ClearEquipment();
		ShowFilteredEquipment(filters);
	}

	private async void OnEquipmentAdded(Equipment equipment)
	{
		ChangeEquipmentInterface changeEquipmentInterface = MonsterHunterIdle.PackedScenes.GetChangeEquipmentInterface(equipment);
		changeEquipmentInterface.Finished += (newEquipment) => equipment = newEquipment;
		AddChild(changeEquipmentInterface);

		await ToSignal(changeEquipmentInterface, ChangeEquipmentInterface.SignalName.Finished);

		RefreshEquipment(equipment);
	}

	private void OnEquipmentUpgraded(Equipment equipment)
	{
		Equipment equipmentToUpgrade;

		if (equipment is Weapon weapon)
		{
			equipmentToUpgrade = HunterManager.FindWeapon(weapon);
		}
		else if (equipment is Armor armor)
		{
			equipmentToUpgrade = HunterManager.FindArmor(armor);
		}
		else
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Find Equipment";
			PrintRich.PrintError(className, message);

			return;
		}

		string previousEquipmentName = equipmentToUpgrade.Name;
		int previousEquipmentSubGrade = equipmentToUpgrade.SubGrade;

		EquipmentManager.UpgradeEquipment(equipmentToUpgrade);
		HunterManager.Equip(equipmentToUpgrade);

		// Console message
		string previousSubGrade = previousEquipmentSubGrade == 0 ? "" : $" (+{previousEquipmentSubGrade})";
		string subGrade = equipmentToUpgrade.SubGrade == 0 ? "" : $" (+{equipmentToUpgrade.SubGrade})";
		string upgradedMessage = $"{previousEquipmentName}{previousSubGrade} Has Been Successfully Upgraded To {equipmentToUpgrade.Name}{subGrade}";
		PrintRich.PrintLine(TextColor.Orange, upgradedMessage);

		RefreshEquipment(equipment);
	}

	private void OnPalicoEquipmentAdded(PalicoEquipment equipment, int index)
	{	
		AddPalicoCraftOptionInterface(equipment, index);
		RefreshEquipment(equipment, index);
	}

	private void OnPalicoEquipmentUpgraded(PalicoEquipment equipment, int index)
	{
		PalicoEquipment equipmentToUpgrade;

		if (equipment is PalicoWeapon targetWeapon)
		{
			List<PalicoWeapon> weapons = PalicoEquipmentManager.FindCraftedWeapons(targetWeapon);
			equipmentToUpgrade = weapons[index];
		}
		else if (equipment is PalicoArmor targetArmor)
		{
			List<PalicoArmor> armor = PalicoEquipmentManager.FindCraftedArmor(targetArmor);
			equipmentToUpgrade = armor[index];
		}
		else
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Find Equipment";
			PrintRich.PrintError(className, message);

			return;
		}

		string previousEquipmentName = equipmentToUpgrade.Name;
		int previousEquipmentSubGrade = equipmentToUpgrade.SubGrade;

		PalicoEquipmentManager.UpgradeEquipment(equipmentToUpgrade);
		PalicoManager.Equip(equipmentToUpgrade);

		PrintRich.PrintEquipmentInfo(TextColor.Pink, equipmentToUpgrade);

		// Console message
		string previousSubGrade = previousEquipmentSubGrade == 0 ? "" : $" (+{previousEquipmentSubGrade})";
		string subGrade = equipmentToUpgrade.SubGrade == 0 ? "" : $" (+{equipmentToUpgrade.SubGrade})";
		string upgradedMessage = $"{previousEquipmentName}{previousSubGrade} Has Been Successfully Upgraded To {equipmentToUpgrade.Name}{subGrade}";
		PrintRich.PrintLine(TextColor.Orange, upgradedMessage);

		RefreshEquipment(equipmentToUpgrade, index);

		PalicoCraftOptionInterface palicoCraftOptionInterface = MonsterHunterIdle.PackedScenes.GetPalicoCraftOptionInterface(equipmentToUpgrade, index);
		AddChild(palicoCraftOptionInterface);
	}

	private void OnCraftButtonPressed(Equipment equipment)
	{
		AddRecipeInterface(equipment);
	}

	private void OnPalicoCraftButtonPressed(PalicoEquipment equipment)
	{
		AddPalicoCraftOptionInterface(equipment);
	}

	private void OnPalicoCraftOptionButtonPressed(PalicoEquipment equipment, bool isCrafting, int index)
	{
		AddRecipeInterface(equipment, isCrafting, index);
	}
	// * END - Signal Functions

	private void AddPalicoCraftOptionInterface(PalicoEquipment equipment, int index = -1)
	{
		PalicoCraftOptionInterface palicoCraftOptionInterface = MonsterHunterIdle.PackedScenes.GetPalicoCraftOptionInterface(equipment, index);
		CallDeferred("add_child", palicoCraftOptionInterface);
	}

	private void AddRecipeInterface(Equipment equipment, bool isCrafting = false, int index = -1)
	{
		if (equipment == null) return;

		RecipeInterface recipeInterface = MonsterHunterIdle.PackedScenes.GetRecipeInterface(equipment, isCrafting, index);
		CallDeferred("add_child", recipeInterface);
	}

	private void RefreshEquipment(Equipment equipment = null, int index = -1)
	{
		ClearEquipment();

		List<Equipment> weapons = [.. EquipmentManager.Weapons];
		ShowEquipment(weapons);

		List<Equipment> armor = [.. EquipmentManager.Armor];
		ShowEquipment(armor);

		List<PalicoEquipment> palicoWeapons = [.. PalicoEquipmentManager.Weapons];
		ShowPalicoEquipment(palicoWeapons);

		List<PalicoEquipment> palicoArmor = [.. PalicoEquipmentManager.Armor];
		ShowPalicoEquipment(palicoArmor);

		if (equipment == null) return;

		if (equipment is not PalicoEquipment)
		{
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.CraftButtonPressed, equipment);
		}
		else
		{
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoCraftOptionButtonPressed, equipment, false, index);
		}
	}

	private void ShowFilteredEquipment(GC.Dictionary<string, bool> filters)
	{
		// Show all the equipment if there are no filters
		bool hasFilter = HasFilter(filters);
		if (!hasFilter)
		{
			List<Equipment> weapons = [.. EquipmentManager.Weapons];
			ShowEquipment(weapons);

			List<Equipment> armor = [.. EquipmentManager.Armor];
			ShowEquipment(armor);
			return;
		}
		
		List<Equipment> filteredEquipment = new List<Equipment>();
		List<Equipment> filteredGroupEquipment = new List<Equipment>();
		foreach (string filterKey in filters.Keys)
		{
			List<Equipment> equipment = [.. EquipmentManager.Weapons];
			equipment.AddRange([.. EquipmentManager.Armor]);

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
					filteredGroupEquipment = [.. filteredGroupEquipment.FindAll(equipment => !EquipmentManager.HasCrafted(equipment))];
					continue;
				}
				if (filteredEquipment.Count > 0)
				{
					filteredEquipment = [.. filteredEquipment.FindAll(equipment => !EquipmentManager.HasCrafted(equipment))];
				}
				else
				{
					filteredEquipment = [.. equipment.FindAll(equipPiece => !EquipmentManager.HasCrafted(equipPiece))];
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
			_craftButtonContainer.AddChild(craftButton);
		}
	}

	private void ShowPalicoEquipment(List<PalicoEquipment> palicoEquipment)
	{
		foreach (PalicoEquipment equipmentPiece in palicoEquipment)
		{
			PalicoCraftButton palicoCraftButton = MonsterHunterIdle.PackedScenes.GetPalicoCraftButton(equipmentPiece);
			_craftButtonContainer.AddChild(palicoCraftButton);
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
}
