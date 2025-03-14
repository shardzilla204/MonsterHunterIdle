using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class MonsterManager : Node
{
	[Export]
	private int _maxRewardCount = 4;

	[Export]
	private MonsterEncounter _encounter;

	private MonstersFileLoader _monstersFileLoader;
	private MonsterLocaleFileLoader _monsterLocaleFileLoader;

	private List<int> _monsterLevels = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
	public MonsterEncounter Encounter => _encounter;

	public List<Monster> Monsters = new List<Monster>();

	public override void _Ready()
	{
		MonsterHunterIdle.Signals.PalicoHunted += (palico) => 
		{
			string palicoDamagedMessage = $"{palico.Name} has damaged the monster";
			PrintRich.Print(TextColor.Yellow, palicoDamagedMessage);
			
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterDamaged, palico.Attack);
		};
	}

	private void SetMonsters()
	{
		GC.Dictionary<string, Variant> monsterDictionaries = _monstersFileLoader.GetDictionary();
		List<string> monsterNames = monsterDictionaries.Keys.ToList();
		foreach (string monsterName in monsterNames)
		{
			GC.Dictionary<string, Variant> monsterDictionary = monsterDictionaries[monsterName].As<GC.Dictionary<string, Variant>>();
			Monster monster = new Monster(monsterName, monsterDictionary);
			monster.Materials.AddRange(GetMonsterMaterials(monster));
			monster.Locales.AddRange(GetMonsterLocales(monster));
			Monsters.Add(monster);
		}
	}

	private List<BiomeType> GetMonsterLocales(Monster monster)
	{
		List<BiomeType> locales = new List<BiomeType>();

		GC.Dictionary<string, Variant> monsterLocaleDictionaries = _monsterLocaleFileLoader.GetDictionary();
		List<string> localeNames = monsterLocaleDictionaries.Keys.ToList();
		foreach (string localeName in localeNames)
		{
			List<string> monsterNames = GetMonsterNamesInLocale(localeName);

			if (!IsMonsterInLocale(monster, monsterNames)) continue;

			BiomeType locale = Enum.Parse<BiomeType>(localeName);
			locales.Add(locale);
		}
		return locales;
	}

	private List<string> GetMonsterNamesInLocale(string localeName)
	{
		return _monsterLocaleFileLoader.GetDictionary()[localeName].As<GC.Array<string>>().ToList();
	}

	public bool IsMonsterInLocale(Monster monster, List<string> monsterNames)
	{
		string monsterName = monsterNames?.Find(monsterName => monsterName == monster.Name);

		return !string.IsNullOrEmpty(monsterName);
	}

	public Monster FindMonster(string monsterName)
	{
		return Monsters.Find(monster => monster.Name == monsterName);
	}

	public List<Monster> GetBiomeMonsters(BiomeType biomeType)
	{
		List<Monster> biomeMonsters = new List<Monster>();
		foreach (Monster monster in Monsters)
		{
			BiomeType? locale = monster.Locales?.Find(locale => locale == biomeType);

			if (locale is null) continue;

			biomeMonsters.Add(monster);
		}
		return biomeMonsters;
	}

	public Monster GetRandomMonster()
	{
		Biome biome = MonsterHunterIdle.BiomeManager.Biome;
		List<Monster> biomeMonsters = MonsterHunterIdle.MonsterManager.GetBiomeMonsters(biome.Type);
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int monsterID = RNG.RandiRange(0, biomeMonsters.Count - 1);

		Monster monster = biomeMonsters[monsterID];
		monster.Level = GetMonsterLevel();

		return monster;
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
	private List<int> GetMonsterLevels() => GameManager.Instance.Player.HunterRank switch
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

	public List<MonsterMaterial> GetMonsterMaterials(Monster monster)
	{
		List<MonsterMaterial> monsterMaterials = new List<MonsterMaterial>();
		List<MonsterMaterial> levelMaterials = FindMonsterMaterialsByLevel(monster);

		for (int i = 0; i < _maxRewardCount; i++)
		{
			MonsterMaterial monsterMaterial = GetMonsterMaterial(levelMaterials);

			int lootValue = GetLootValue(levelMaterials);
			while (lootValue > 0)
			{
				lootValue -= monsterMaterial.Rarity;
				monsterMaterial = GetMonsterMaterial(levelMaterials);
			}

			monsterMaterials.Add(monsterMaterial);
		}
		return monsterMaterials;
	}

	public List<MonsterMaterial> FindMonsterMaterialsByLevel(Monster monster)
	{
		List<MonsterMaterial> monsterMaterials = MonsterHunterIdle.MaterialManager.FindMonsterMaterials(monster);

		return monsterMaterials.FindAll(material => GetLevelRarity(material.Rarity) <= GetLevelRarity(monster.Level));
	}

	private MonsterMaterial GetMonsterMaterial(List<MonsterMaterial> monsterMaterials)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int materialID = RNG.RandiRange(0, monsterMaterials.Count - 1);

		return monsterMaterials[materialID];
	}

	private int GetLootValue(List<MonsterMaterial> monsterMaterials)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int materialRarityTotal = GetMaterialRarityTotal(monsterMaterials);
		int lootValue = RNG.RandiRange(0, materialRarityTotal);

		return lootValue;
	}

	private int GetMaterialRarityTotal(List<MonsterMaterial> monsterMaterials)
	{
		int materialRarityTotal = 0;
		foreach (MonsterMaterial monsterMaterial in monsterMaterials)
		{
			materialRarityTotal += monsterMaterial.Rarity;
		}
		return materialRarityTotal;
	}
}
