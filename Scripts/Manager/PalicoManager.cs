using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class PalicoManager : Node
{
	[Export(PropertyHint.Range, "1, 10, 0.5")]
	private float _actionIntervalSeconds = 5f;

	[Export]
	private int _startingPalicoCount = 0;

	public static List<Palico> Palicos = new List<Palico>();
	public static int MaxPalicoCount = 4;

	public static float ActionIntervalSeconds = 5;
	public static int StartingPalicoCount = 0;

	public override void _EnterTree()
	{
		ActionIntervalSeconds = _actionIntervalSeconds;
		StartingPalicoCount = _startingPalicoCount;
	}

	public override void _Ready()
	{
		ActionInterval();
	}

	public static string GetRandomName()
	{
		string[] names = ["Matthew", "Jonathan", "Aiden", "Nathan", "Kristian", "Jaydon", "Brenyn", "Eyan", "Mindy", "Carrigan"];
		int nameCount = names.Length - 1;
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int randomIndex = RNG.RandiRange(0, nameCount);

		return names[randomIndex];
	}

	private static void AddStartingPalicos()
	{
		for (int i = 0; i < StartingPalicoCount; i++)
		{
			Palico palico = new Palico();
			palico.Name = GetRandomName();
			Palicos.Add(palico);
		}
	}

	private async void ActionInterval()
	{
		while (true)
		{
			await ToSignal(GetTree().CreateTimer(_actionIntervalSeconds), SceneTreeTimer.SignalName.Timeout);

			foreach (Palico palico in Palicos)
			{
				// Attack if monster is present or gather materials
				if (MonsterManager.Encounter.IsInEncounter)
				{
					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoHunted, palico);
					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterDamaged, palico.Weapon.Attack); // ! Change later
				}
				else
				{
					LocaleMaterial localeMaterial = LocaleManager.GetLocaleMaterial();

					// Console message
					string palicoGatheredMessage = $"{palico.Name} Has Picked Up {localeMaterial.Name}";
					PrintRich.PrintLine(TextColor.Yellow, palicoGatheredMessage);

					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoGathered, palico);
					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.LocaleMaterialAdded, localeMaterial);
				}
			}
		}
	}

	public static bool IsEquipped(Palico palico, PalicoEquipment equipment, int index)
	{
		bool isEquipped = false;
		if (equipment is PalicoWeapon targetWeapon)
		{
			if (palico.Weapon.Name == "") return false;

			List<PalicoWeapon> weapons = PalicoEquipmentManager.FindCraftedWeapons(targetWeapon);
			PalicoWeapon desiredWeapon = weapons[index];

			isEquipped = palico.Weapon == desiredWeapon;
		}
		else if (equipment is PalicoArmor targetArmor)
		{
			PalicoArmor armor = GetArmor(palico, targetArmor);
			if (armor.Name == "") return false;

			List<PalicoArmor> armorPieces = PalicoEquipmentManager.FindCraftedArmor(armor);
			PalicoArmor desiredArmor = armorPieces[index];

			isEquipped = armor == desiredArmor;
		}
		return isEquipped;
	}

	public static void Equip(Palico palico, PalicoEquipment equipment)
	{
		if (equipment is PalicoWeapon weapon)
		{
			palico.Weapon = weapon;
		}
		else if (equipment is PalicoArmor armor)
		{
			if (armor.Type == PalicoEquipmentType.Head)
			{
				palico.Head = armor;
			}
			else if (armor.Type == PalicoEquipmentType.Chest)
			{
				palico.Chest = armor;
			}
		}

		string equipMessage = $"{palico.Name} Has Equipped {equipment.Name}";
		PrintRich.PrintLine(TextColor.Orange, equipMessage);
	}

	// Create an "empty" object
	public static void Unequip(Palico palico, PalicoEquipment equipment)
	{
		if (equipment is PalicoWeapon)
		{
			palico.Weapon = new PalicoWeapon();
		}
		else if (equipment is PalicoArmor armor)
		{
			if (armor.Type == PalicoEquipmentType.Head)
			{
				palico.Head = new PalicoArmor(PalicoEquipmentType.Head);
			}
			else if (armor.Type == PalicoEquipmentType.Chest)
			{
				palico.Chest = new PalicoArmor(PalicoEquipmentType.Chest);
			}
		}

		string equipMessage = $"{palico.Name} Has Equipped {equipment.Name}";
		PrintRich.PrintLine(TextColor.Orange, equipMessage);
	}

	private static PalicoArmor GetArmor(Palico palico, PalicoArmor armor)
	{
		switch (armor.Type)
		{
			case PalicoEquipmentType.Head:
				return palico.Head;
			case PalicoEquipmentType.Chest:
				return palico.Chest;
			default:
				return null;
		}
	}

	// For upgrading equipment 
	public static void Equip(PalicoEquipment equipment)
	{
		Palico palico = null;

		if (equipment is PalicoWeapon weapon)
		{
			palico = Palicos.Find(palico => palico.Weapon == weapon);
			if (palico == null) return;

			palico.Weapon = weapon;
		}
		else if (equipment is PalicoArmor armor)
		{
			if (armor.Type == PalicoEquipmentType.Head)
			{
				palico = Palicos.Find(palico => palico.Head == armor);
				if (palico == null) return;

				palico.Head = armor;
			}
			else if (armor.Type == PalicoEquipmentType.Chest)
			{
				palico = Palicos.Find(palico => palico.Chest == armor);
				if (palico == null) return;

				palico.Chest = armor;
			}
		}

		string equipMessage = $"{palico.Name} Has Equipped {equipment.Name}";
		PrintRich.PrintLine(TextColor.Orange, equipMessage);
	}

	// * START - Data Methods
	public static GC.Array<GC.Dictionary<string, Variant>> GetData()
	{
		GC.Array<GC.Dictionary<string, Variant>> palicosData = new GC.Array<GC.Dictionary<string, Variant>>();
		foreach (Palico palico in Palicos)
		{
			palicosData.Add(GetPalicoData(palico));
		}
		return palicosData;
	}

	public static GC.Dictionary<string, Variant> GetPalicoData(Palico palico)
	{
		GC.Dictionary<string, Variant> palicoData = new GC.Dictionary<string, Variant>()
		{
			{ "Name", palico.Name },
			{ "Weapon", PalicoEquipmentManager.GetWeaponData(palico.Weapon) },
			{ "Head", PalicoEquipmentManager.GetArmorPieceData(palico.Head) },
			{ "Chest", PalicoEquipmentManager.GetArmorPieceData(palico.Chest) }
		};
		return palicoData;
	}

	public static void SetData(GC.Array<GC.Dictionary<string, Variant>> palicosData)
	{
		foreach (GC.Dictionary<string, Variant> palicoData in palicosData)
		{
			Palico palico = GetPalicoFromData(palicoData);
			Palicos.Add(palico);
		}
	}

	public static Palico GetPalicoFromData(GC.Dictionary<string, Variant> palicoData)
	{
		Palico palico = new Palico();
		palico.Name = palicoData["Name"].As<string>();

		GC.Dictionary<string, Variant> weaponData = palicoData["Weapon"].As<GC.Dictionary<string, Variant>>();
		palico.Weapon = PalicoEquipmentManager.GetWeaponFromData(weaponData);

		GC.Dictionary<string, Variant> headData = palicoData["Head"].As<GC.Dictionary<string, Variant>>();
		palico.Head = PalicoEquipmentManager.GetArmorPieceFromData(headData);

		GC.Dictionary<string, Variant> chestData = palicoData["Chest"].As<GC.Dictionary<string, Variant>>();
		palico.Chest = PalicoEquipmentManager.GetArmorPieceFromData(chestData);
		GD.Print(palico.Head == null);
		GD.Print(palico.Head.Name == "");

		return palico;
	}

	public static void DeleteData()
	{
		Palicos.Clear();

		AddStartingPalicos();
	}
	// * END - Data Methods
}
