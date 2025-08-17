using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public enum MonsterRarity
{
	One = 1000,
	Two = 500,
	Three = 250,
	Four = 125,
	Five = 50,
	Six = 25
}

public partial class MonsterManager : Node
{
	[Export]
	private int _maxRewardCount = 4;

	[Export]
	private MonsterEncounter _encounter;

	private List<int> _monsterLevels = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

	public int MaxRewardCount = 4;
	public MonsterEncounter Encounter;

	public List<Monster> Monsters = new List<Monster>();
	public List<MonsterMaterial> Materials = new List<MonsterMaterial>();

	public override void _EnterTree()
	{
		MonsterHunterIdle.MonsterManager = this;

		MaxRewardCount = _maxRewardCount;
		Encounter = _encounter;

		MonsterHunterIdle.Signals.PalicoHunted += (palico) =>
		{
			// Console message
			string palicoDamagedMessage = $"{palico.Name} Has Damaged The Monster";
			PrintRich.Print(TextColor.Yellow, palicoDamagedMessage);

			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterDamaged, palico.Attack);
		};

		LoadMonsters();
		LoadMaterials();

		PrintRich.PrintMonsters(TextColor.Blue);
	}

	private void LoadMonsters()
	{
		string fileName = "Monsters";
		GC.Dictionary<string, Variant> monsterData = MonsterHunterIdle.LoadFile(fileName).As<GC.Dictionary<string, Variant>>();
		if (monsterData == null) return;

		List<GC.Dictionary<string, Variant>> monsterDictionaries = monsterData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
		
		// Set monsters
		foreach (GC.Dictionary<string, Variant> dictionary in monsterDictionaries)
		{
			Monster monster = new Monster(dictionary);
			Monsters.Add(monster);
		}
	}

	private void LoadMaterials()
	{
		string fileName = "MonsterMaterials";
		GC.Dictionary<string, Variant> monsterMaterialData = MonsterHunterIdle.LoadFile(fileName).As<GC.Dictionary<string, Variant>>();
		if (monsterMaterialData == null) return;

		List<GC.Dictionary<string, Variant>> monsterMaterialDictionaries = monsterMaterialData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();

		// Set monster materials
		foreach (GC.Dictionary<string, Variant> dictionary in monsterMaterialDictionaries)
		{
			MonsterMaterial monsterMaterial = new MonsterMaterial(dictionary);
			Materials.Add(monsterMaterial);
		}
	}
	
	public List<Monster> GetLocaleMonsters(LocaleType localeType)
	{
		List<Monster> localeMonsters = new List<Monster>();
		foreach (Monster monster in Monsters)
		{
			LocaleType? locale = monster.Locales?.Find(locale => locale == localeType);

			if (locale is null) continue;

			localeMonsters.Add(monster);
		}
		return localeMonsters;
	}

	public Monster GetRandomMonster()
	{
		Locale locale = MonsterHunterIdle.LocaleManager.Locale;
		List<Monster> localeMonsters = MonsterHunterIdle.MonsterManager.GetLocaleMonsters(locale.Type);
		RandomNumberGenerator RNG = new RandomNumberGenerator();

		try 
		{
			int monsterID = RNG.RandiRange(0, localeMonsters.Count - 1);
			Monster monster = localeMonsters[monsterID];

			Monster monsterClone = new Monster();
			int monsterlevel = GetMonsterLevel();
			monsterClone.Clone(monster, monsterlevel);

			return monsterClone;
		}
		catch (Exception)
		{
			string errorMessage = $"Locale Monsters Count: {localeMonsters.Count}";
			PrintRich.PrintLine(TextColor.Red, errorMessage);

			return null;
		}
	}

	private int GetMonsterLevel()
	{
		List<int> monsterLevels = GetMonsterLevels();

		int monsterlevelRarityTotal = GetMonsterLevelRarityTotal(monsterLevels);
		int randomMonsterLevelRarity = GetRandomMonsterLevelRarity(monsterlevelRarityTotal);
		int randomMonsterlevel = GetRandomMonsterLevel(monsterLevels);

		while (randomMonsterLevelRarity > 0)
		{
			randomMonsterLevelRarity -= GetLevelRarity(randomMonsterlevel);
			randomMonsterlevel = GetRandomMonsterLevel(monsterLevels);
		}
		return randomMonsterlevel;
	}

	private int GetRandomMonsterLevel(List<int> monsterLevels)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int randomMonsterlevel = RNG.RandiRange(0, monsterLevels.Count - 1);

		return randomMonsterlevel + 1;
	}

	// Gets levels that based on the player's current rank
	private List<int> GetMonsterLevels() => MonsterHunterIdle.HunterManager.Hunter.Rank switch
	{
		< 5 => FindLevels(2),
		< 10 => FindLevels(3),
		< 15 => FindLevels(4),
		< 20 => FindLevels(5),
		< 25 => FindLevels(6),
		< 35 => FindLevels(7),
		< 50 => FindLevels(8),
		< 75 => FindLevels(9),
		< 100 => FindLevels(10),
		_ => FindLevels(2)
	};

	// Finds and returns a list that has the current level and levels below 
	// { eg: if (monster level == 3) return 1, 2, & 3 }
	private List<int> FindLevels(int monsterLevel)
	{
		return _monsterLevels.FindAll(level => level <= monsterLevel);
	}

	public int GetLevelRarity(int monsterLevel) => monsterLevel switch
	{
		1 => 2500,
		2 => 2000,
		3 => 1500,
		4 => 1000,
		5 => 500,
		6 => 250,
		7 => 200,
		8 => 150,
		9 => 100,
		10 => 50,
		_ => 2500
	};

	private int GetRandomMonsterLevelRarity(int monsterLevelRarityTotal)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int randomMonsterLevelRarity = RNG.RandiRange(0, monsterLevelRarityTotal);

		return randomMonsterLevelRarity;
	}

	private int GetMonsterLevelRarityTotal(List<int> monsterLevels)
	{
		int monsterLevelRarityTotal = 0;
		foreach (int monsterLevel in monsterLevels)
		{
			monsterLevelRarityTotal += GetLevelRarity(monsterLevel);
		}
		return monsterLevelRarityTotal;
	}

	public List<MonsterMaterial> FindMonsterMaterialsByLevel(Monster monster)
	{
		return Materials.FindAll(material => GetLevelRarity(material.Rarity) <= GetLevelRarity(monster.Level));
	}

	public float GetMonsterHealth(Monster monster)
	{
		float multiplier = 1.65f;
		return monster.Level * monster.Health * multiplier;
	}
}
