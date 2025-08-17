using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class MonsterEncounter : Node
{
	[Export]
	private float _maxEncounterChance = 100;

	[Export(PropertyHint.Range, "1, 25, 1")]
	private float _encounterIncrease = 1;

	private float _encounterChance = 0;

	public int Time;
	public int Health;
	
	public bool IsInEncounter = false;
	
	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.LocaleMaterialAdded += (localeMaterial) => IncreaseEncounterChance();
	}

   	private async void IncreaseEncounterChance()
	{
		if (IsInEncounter) return;

		_encounterChance += _encounterIncrease;

		string encounterChanceIncreasedMessage = $"An Encounter Has A {_encounterChance}% To Occur";
		PrintRich.PrintLine(TextColor.Orange, encounterChanceIncreasedMessage);

		bool hasEncounter = HasEncounter();
		if (hasEncounter)
		{
			string hasEncounteredMessage = $"An Encounter Has Occured";
			PrintRich.PrintLine(TextColor.Orange, hasEncounteredMessage);

			SetMonsterEncounter();
			IsInEncounter = true;
			await ToSignal(MonsterHunterIdle.Signals, Signals.SignalName.MonsterEncounterFinished);
			IsInEncounter = false;
		}
	}

	private bool HasEncounter()
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		float currentChance = _maxEncounterChance - _encounterChance;
		float randomChance = RNG.RandfRange(_encounterChance, _maxEncounterChance);

		return randomChance >= currentChance;
	}

	private void SetMonsterEncounter()
	{
		_encounterChance = 0;

		Monster monster = MonsterHunterIdle.MonsterManager.GetRandomMonster();
		if (monster == null) return;

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterEncountered, monster);
	}

	public void GetEncounterRewards(Monster monster)
	{
		int pointsAmount = GetHunterPointsReward(monster);
		MonsterHunterIdle.HunterManager.AddHunterPoints(pointsAmount);

		int zennyAmount = GetZennyReward(monster);
		MonsterHunterIdle.HunterManager.AddZenny(zennyAmount);

		List<MonsterMaterial> materialRewards = GetMaterialRewards(monster);
		foreach (MonsterMaterial materialReward in materialRewards)
		{
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterMaterialAdded, materialReward);
		}

		PrintRich.PrintEncounterRewards(pointsAmount, zennyAmount, materialRewards);
	}

	private List<MonsterMaterial> GetMaterialRewards(Monster targetMonster)
	{
		List<MonsterMaterial> monsterMaterials = GetMonsterMaterials(targetMonster);

		List<MonsterMaterial> materialRewards = new List<MonsterMaterial>();
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		for (int i = 0; i < MonsterHunterIdle.MonsterManager.MaxRewardCount; i++)
		{
			int randomIndex = RNG.RandiRange(0, monsterMaterials.Count - 1);
			materialRewards.Add(monsterMaterials[randomIndex]);
		}
		return materialRewards;
	}

	private List<MonsterMaterial> GetMonsterMaterials(Monster targetMonster)
	{
		List<MonsterMaterial> monsterMaterials = new List<MonsterMaterial>();
		foreach (MonsterMaterial material in MonsterHunterIdle.MonsterManager.Materials)
		{
			bool hasMonster = material.Monsters.Contains(targetMonster.Name);
			if (hasMonster) monsterMaterials.Add(material);
		}
		return monsterMaterials.FindAll(material => material.Rarity <= targetMonster.Level);;
	}

	private int GetHunterPointsReward(Monster monster) => monster.Level switch
	{
		1 => 10,
		2 => 20,
		3 => 40,
		4 => 60,
		5 => 80,
		6 => 100,
		7 => 110,
		8 => 120,
		9 => 130,
		10 => 150,
		_ => 10
	};

	private int GetZennyReward(Monster monster) => monster.Level switch
	{
		1 => 10,
		2 => 20,
		3 => 30,
		4 => 40,
		5 => 50,
		6 => 100,
		7 => 110,
		8 => 130,
		9 => 140,
		10 => 150,
		_ => 10,
	};
}
