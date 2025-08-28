using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class PalicoManager : Node
{
	[Export(PropertyHint.Range, "1, 10, 0.5")]
	private float _actionIntervalSeconds = 5f;

	public static List<Palico> Palicos = new List<Palico>();
	public static int MaxPalicoCount = 4;

	public static PalicoEquipmentManager Equipment;

	public static float ActionIntervalSeconds = 5;

	public override void _EnterTree()
	{
		ActionIntervalSeconds = _actionIntervalSeconds;
	}

	public override void _Ready()
	{
		ActionInterval();
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
					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterDamaged, 5); // ! Change later
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
			// { "Weapon", EquipmentManager.GetWeaponData(palico.Weapon) },
			// { "Head", EquipmentManager.GetArmorPieceData(palico.Head) },
			// { "Chest", EquipmentManager.GetArmorPieceData(palico.Chest) },
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
		// PalicoWeapon weapon = 
		// PalicoArmor
		// PalicoArmor
		return palico;
	}

	public static void DeleteData()
	{
		Palicos.Clear();
		Equipment.CraftedWeapons.Clear();
		Equipment.CraftedArmor.Clear();
	}
}
