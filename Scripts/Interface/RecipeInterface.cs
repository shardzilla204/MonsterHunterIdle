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

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.CraftButtonPressed -= OnCraftButtonPressed;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.CraftButtonPressed += OnCraftButtonPressed;
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

	public void SetMaterials(Equipment equipment)
	{
		_equipment = equipment;

		string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";

		// Console message
		string recipeMessage = $"Showing Recipe For: {equipment.Name}{subGrade}";
		PrintRich.PrintLine(TextColor.Yellow, recipeMessage);

		bool hasCrafted = EquipmentManager.HasCrafted(equipment);
		List<GC.Dictionary<string, Variant>> recipe = EquipmentManager.GetRecipe(equipment, hasCrafted);

		foreach (GC.Dictionary<string, Variant> materialDictionary in recipe)
		{
			string name = materialDictionary["Name"].As<string>();
			int amount = materialDictionary["Amount"].As<int>();

			Material material = MonsterHunterIdle.FindMaterial(name);
			CraftingMaterialLog craftingMaterialLog = MonsterHunterIdle.PackedScenes.GetCraftingMaterialLog(material, amount);
			_craftingMaterialLogContainer.AddChild(craftingMaterialLog);
		}

		_craftingLabel.Text = hasCrafted ? "Upgrade" : "Forge";
	}

	private void OnCraftButtonPressed(Equipment equipment)
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

		bool hasCrafted = EquipmentManager.HasCrafted(_equipment);
		if (hasCrafted)
		{
			// Upgrade equipment
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.EquipmentUpgraded, _equipment);
		}
		else
		{
			// Craft equipment
			if (_equipment is Weapon weapon)
			{
				EquipmentManager.CraftedWeapons.Add(weapon);
				MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.WeaponAdded, weapon);
			}
			else if (_equipment is Armor armor)
			{
				EquipmentManager.CraftedArmor.Add(armor);
				MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.ArmorAdded, armor);
			}
			QueueFree();
		}
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
