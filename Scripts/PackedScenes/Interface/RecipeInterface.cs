using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class RecipeInterface : NinePatchRect
{
	[Export]
	private CustomButton _acceptButton;

	[Export]
	private CustomButton _cancelButton;

	[Export]
	private Label _craftingLabel;

	[Export]
	private Label _craftingCostLabel;

	[Export]
	private Container _craftingMaterialLogContainer;

	private Equipment _equipment;
	private int _craftingCost = 0;
	private bool _isUpgrading = false;

	private int _index = -1; // For Palico equipment

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.CraftButtonPressed -= OnCraftButtonPressed;
		MonsterHunterIdle.Signals.PalicoCraftButtonPressed -= OnCraftButtonPressed;
		MonsterHunterIdle.Signals.PalicoCraftOptionButtonPressed -= OnPalicoCraftOptionButtonPressed;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.CraftButtonPressed += OnCraftButtonPressed;
		MonsterHunterIdle.Signals.PalicoCraftButtonPressed += OnCraftButtonPressed;
		MonsterHunterIdle.Signals.PalicoCraftOptionButtonPressed += OnPalicoCraftOptionButtonPressed;
	}

	public override void _Ready()
	{
		_acceptButton.Pressed += OnAcceptButtonPressed;
		_cancelButton.Pressed += QueueFree;

		bool hasCrafted = EquipmentManager.HasCrafted(_equipment);
		int grade = _equipment.Grade;
		int subGrade = hasCrafted ? _equipment.SubGrade + 1 : _equipment.SubGrade;
		_craftingCost = EquipmentManager.GetCraftingCost(grade, subGrade);

		_craftingCostLabel.Text = $"{_craftingCost}z";
	}
	
	// * START - Signal Methods
	private void OnCraftButtonPressed(Equipment equipment)
	{
		QueueFree();
	}

	private void OnPalicoCraftOptionButtonPressed(PalicoEquipment equipment, bool isCrafting, int index)
	{
		QueueFree();
	}

	private void OnAcceptButtonPressed()
	{
		if (Hunter.Zenny < _craftingCost) return;

		bool hasMaterials = HasMaterials();
		if (!hasMaterials) return;

		Hunter.Zenny -= _craftingCost;

		// Subtract materials from item box
		foreach (CraftingMaterialLog craftingMaterialLog in _craftingMaterialLogContainer.GetChildren())
		{
			Material material = craftingMaterialLog.Material;
			int amount = craftingMaterialLog.Amount;
			ItemBox.SubtractMaterial(material, amount);
		}

		if (_isUpgrading)
		{
			if (_equipment is not PalicoEquipment)
			{
				MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.EquipmentUpgraded, _equipment);
			}
			else
			{
				MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoEquipmentUpgraded, _equipment, _index);
			}
		}
		else
		{
			AddEquipment();
		}
	}
	// * END - Signal Methods

	public void SetMaterials(Equipment equipment, bool isCrafting, int index)
	{
		_equipment = equipment;
		_index = index;

		string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";

		// Console message
		string recipeMessage = $"Showing Recipe For: {equipment.Name}{subGrade}";
		PrintRich.PrintLine(TextColor.Yellow, recipeMessage);

		// If crafting, don't get the next recipe
		List<GC.Dictionary<string, Variant>> recipe;
		if (equipment is PalicoEquipment palicoEquipment)
		{
			recipe = PalicoEquipmentManager.GetEquipmentRecipe(palicoEquipment, !isCrafting);
		}
		else
		{
			isCrafting = !EquipmentManager.HasCrafted(equipment);
			recipe = EquipmentManager.GetEquipmentRecipe(equipment);
		}

		// If the equipment reaches max level
		if (recipe == null)
		{
			QueueFree();
			return;
		}

		// Show the materials listed in the recipe
		foreach (GC.Dictionary<string, Variant> materialDictionary in recipe)
		{
			string name = materialDictionary["Name"].As<string>();
			int amount = materialDictionary["Amount"].As<int>();

			Material material = MonsterHunterIdle.FindMaterial(name);
			CraftingMaterialLog craftingMaterialLog = MonsterHunterIdle.PackedScenes.GetCraftingMaterialLog(material, amount);
			_craftingMaterialLogContainer.AddChild(craftingMaterialLog);
		}

		_isUpgrading = !isCrafting;

		_craftingLabel.Text = isCrafting ? "Forge" : "Upgrade";
	}

	private void AddEquipment()
	{
		if (_equipment is Weapon weapon)
		{
			EquipmentManager.CraftedWeapons.Add(weapon);
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.EquipmentAdded, weapon);
		}
		else if (_equipment is Armor armor)
		{
			EquipmentManager.CraftedArmor.Add(armor);
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.EquipmentAdded, armor);
		}
		// Move index to the equipment that was just forged
		else if (_equipment is PalicoWeapon palicoWeapon)
		{
			PalicoEquipmentManager.CraftedWeapons.Add(palicoWeapon);
			int weaponCount = PalicoEquipmentManager.FindCraftedWeapons(palicoWeapon).Count - 1;
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoEquipmentAdded, palicoWeapon, ++_index + weaponCount);
		}
		else if (_equipment is PalicoArmor palicoArmor)
		{
			PalicoEquipmentManager.CraftedArmor.Add(palicoArmor);
			int armorCount = PalicoEquipmentManager.FindCraftedArmor(palicoArmor).Count - 1;
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoEquipmentAdded, palicoArmor, ++_index + armorCount);
		}
		QueueFree();
	}

	private bool HasMaterials()
	{
		bool hasMaterials = true;
		foreach (CraftingMaterialLog craftingMaterialLog in _craftingMaterialLogContainer.GetChildren())
		{
			Material requiredMaterial = craftingMaterialLog.Material;
			int requiredAmount = craftingMaterialLog.Amount;

			List<Material> targetMaterials = ItemBox.FindAllMaterial(requiredMaterial.Name);
			if (targetMaterials.Count < requiredAmount) return false;
		}

		return hasMaterials;
	}
}
