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
	private Container _craftingMaterialLogContainer;

	private Equipment _equipment;

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
	}

	public void SetMaterials(Equipment equipment)
	{
		_equipment = equipment;

		string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";

		// Console message
		string recipeMessage = $"Showing Recipe For: {equipment.Name}{subGrade}";
		PrintRich.PrintLine(TextColor.Yellow, recipeMessage);

		bool hasCrafted = MonsterHunterIdle.EquipmentManager.HasCrafted(equipment);
		List<GC.Dictionary<string, Variant>> recipe = MonsterHunterIdle.EquipmentManager.FindRecipe(equipment, hasCrafted);

		foreach (GC.Dictionary<string, Variant> materialDictionary in recipe)
		{
			string name = materialDictionary["Name"].As<string>();
			int amount = materialDictionary["Amount"].As<int>();

			Material material = MonsterHunterIdle.FindMaterial(name);
			CraftingMaterialLog craftingMaterialLog = MonsterHunterIdle.PackedScenes.GetCraftingMaterialLog(material, amount);
			_craftingMaterialLogContainer.AddChild(craftingMaterialLog);
		}

		_acceptButton.Text = hasCrafted ? "Upgrade" : "Forge";
	}

	private void OnCraftButtonPressed(Equipment equipment)
	{
		QueueFree();
	}

	private void OnAcceptButtonPressed()
	{
		bool hasMaterials = HasMaterials();
		if (!hasMaterials) return;

		// Subtract materials from item box
		foreach (CraftingMaterialLog craftingMaterialLog in _craftingMaterialLogContainer.GetChildren())
		{
			Material material = craftingMaterialLog.Material;
			int amount = craftingMaterialLog.Amount;
			MonsterHunterIdle.ItemBox.SubtractMaterial(material, amount);
		}

		bool hasCrafted = MonsterHunterIdle.EquipmentManager.HasCrafted(_equipment);
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
				MonsterHunterIdle.EquipmentManager.CraftedWeapons.Add(weapon);
				MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.WeaponAdded, weapon);
			}
			else if (_equipment is Armor armor)
			{
				MonsterHunterIdle.EquipmentManager.CraftedArmor.Add(armor);
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

			List<Material> targetMaterials = MonsterHunterIdle.ItemBox.FindAllMaterial(requiredMaterial.Name);
			if (targetMaterials.Count < requiredAmount) return false;
		}

		return hasMaterials;
	}
}
