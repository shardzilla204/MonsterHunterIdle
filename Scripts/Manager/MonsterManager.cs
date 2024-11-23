using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MonsterHunterIdle;

public partial class MonsterManager : Node
{
	[Signal]
	public delegate void EncounteredEventHandler(MonsterData monster);

	private static MonsterManager _instance;
	public static MonsterManager Instance
	{
		get => _instance;
		private set 
		{
			if (_instance == null)
			{
				_instance = value;
			}
			else if (_instance != value)
			{
				GD.PrintRich($"{nameof(MonsterManager)} already exists");
			}
		}
	}

	[Export]
	public MonsterData Monster;

	[Export]
	private float _maxEncounterChance = 100f;

	[Export]
	private int _maxRewardCount = 4;

	private float _encounterChance = 0f;

	private List<MonsterLevel> _monsterLevels = new List<MonsterLevel>()
	{
		MonsterLevel.One,
		MonsterLevel.Two,
		MonsterLevel.Three,
		MonsterLevel.Four,
		MonsterLevel.Five,
		MonsterLevel.Six,
		MonsterLevel.Seven,
		MonsterLevel.Eight,
		MonsterLevel.Nine,
		MonsterLevel.Ten
	};

	public override void _Ready()
	{
		Instance = this;
	}

	public void CheckEncounter()
	{
		_encounterChance++;

		RandomNumberGenerator RNG = new RandomNumberGenerator();
		float currentChance = _maxEncounterChance - _encounterChance;
		float randomChance = RNG.RandfRange(_encounterChance, _maxEncounterChance);

		if (randomChance < currentChance) return;

		SetEncounter();
	}

	private void SetEncounter()
	{
		_encounterChance = 0f;

		MonsterData monster = GetMonster();
		Monster = monster;

		EmitSignal(SignalName.Encountered, monster);
	}

	private MonsterData GetMonster()
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		BiomeData biome = BiomeManager.Instance.Biome;
		int monsterID = RNG.RandiRange(0, biome.Monsters.Count - 1);

		MonsterData monster = biome.Monsters[monsterID];
		monster.SetValues();
		monster.Level = GetMonsterLevel();

		return monster;
	}

	private MonsterLevel GetMonsterLevel()
	{
		List<MonsterLevel> metLevels = GetMetLevels(GameManager.Instance.Player.HunterRank);

		int levelTotal = GetLevelTotal(metLevels);
		int randomValue = GetRandomLevelValue(levelTotal);
		MonsterLevel level = GetRandomLevel(metLevels);
		
		while (randomValue > 0)
		{
			randomValue -= (int) level;
			level = GetRandomLevel(metLevels);
		}
		return level;
	}

	private MonsterLevel GetRandomLevel(List<MonsterLevel> metLevels)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int levelID = RNG.RandiRange(0, metLevels.Count - 1);
		return metLevels[levelID];
	}

	// Gets levels that based on the player's current rank
	private List<MonsterLevel> GetMetLevels(int playerRank) => playerRank switch
	{
		< 5 => FindLevels(MonsterLevel.Two),
		< 10 => FindLevels(MonsterLevel.Three),
		< 15 => FindLevels(MonsterLevel.Four),
		< 20 => FindLevels(MonsterLevel.Five),
		< 25 => FindLevels(MonsterLevel.Six),
		< 35 => FindLevels(MonsterLevel.Seven),
		< 50 => FindLevels(MonsterLevel.Eight),
		< 75 => FindLevels(MonsterLevel.Nine),
		< 100 => FindLevels(MonsterLevel.Ten),
		_ => FindLevels(MonsterLevel.Two)
	};

	// Finds and returns a list that has the current level and levels below 
	// { eg: if (monster level == 3) return 1, 2, & 3 }
	private List<MonsterLevel> FindLevels(MonsterLevel monsterLevel)
	{
		return _monsterLevels.FindAll(level => GetLevelValue(level) <= GetLevelValue(monsterLevel));
	}

	// Gets the level value 
	// { eg: MonsterLevel.Four = 4 }
	private int GetLevelValue(MonsterLevel monsterLevel) => monsterLevel switch
	{
		MonsterLevel.One => 1,
		MonsterLevel.Two => 2,
		MonsterLevel.Three => 3,
		MonsterLevel.Four => 4,
		MonsterLevel.Five => 5,
		MonsterLevel.Six => 6,
		MonsterLevel.Seven => 7,
		MonsterLevel.Eight => 8,
		MonsterLevel.Nine => 9,
		MonsterLevel.Ten => 10,
		_ => 1,
	};

	public int GetLevelValue() => Monster.Level switch
	{
		MonsterLevel.One => 1,
		MonsterLevel.Two => 2,
		MonsterLevel.Three => 3,
		MonsterLevel.Four => 4,
		MonsterLevel.Five => 5,
		MonsterLevel.Six => 6,
		MonsterLevel.Seven => 7,
		MonsterLevel.Eight => 8,
		MonsterLevel.Nine => 9,
		MonsterLevel.Ten => 10,
		_ => 1,
	};

	private int GetRandomLevelValue(int levelTotal)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		return RNG.RandiRange(0, levelTotal);
	}

	private int GetLevelTotal(List<MonsterLevel> levels)
	{
		int levelTotal = 0;
		foreach (MonsterLevel level in levels)
		{
			levelTotal += (int) level;
		}
		return levelTotal;
	}

	public void GetRewards()
	{
		GetMaterials(); //!
		GameManager.Instance.GetRewards(GetProgressAmount(), GetZennyAmount());
	}

	private int GetProgressAmount() => Monster.Level switch
	{
		MonsterLevel.One => 10,
		MonsterLevel.Two => 20,
		MonsterLevel.Three => 40,
		MonsterLevel.Four => 60,
		MonsterLevel.Five => 80,
		MonsterLevel.Six => 100,
		MonsterLevel.Seven => 110,
		MonsterLevel.Eight => 120,
		MonsterLevel.Nine => 130,
		MonsterLevel.Ten => 150,
		_ => 10
	};

	private int GetZennyAmount() => Monster.Level switch
	{
		MonsterLevel.One => 10,
		MonsterLevel.Two => 20,
		MonsterLevel.Three => 30,
		MonsterLevel.Four => 40,
		MonsterLevel.Five => 50,
		MonsterLevel.Six => 100,
		MonsterLevel.Seven => 110,
		MonsterLevel.Eight => 130,
		MonsterLevel.Nine => 140,
		MonsterLevel.Ten => 150,
		_ => 10,
	};

	private void GetMaterials()
	{
		List<MonsterMaterialData> levelMaterials = GetLevelMaterials();
		List<MonsterMaterialData> monsterMaterials = new List<MonsterMaterialData>();

		for (int i = 0; i < _maxRewardCount; i++)
		{
			MonsterMaterialData material = GetMaterial(levelMaterials); //!

			int lootValue = GetLootValue(levelMaterials);
			while (lootValue > 0)
			{
				lootValue -= (int) material.Rarity;
				material = GetMaterial(levelMaterials);
			}

			monsterMaterials.Add(material);
		}
		AddMaterials(monsterMaterials);
	}

	private List<MonsterMaterialData> GetLevelMaterials()
	{
		return Monster.Materials.ToList().FindAll(material => GetLevelValue(material.MonsterLevel) <= GetLevelValue(Monster.Level));
	}

	private MonsterMaterialData GetMaterial(List<MonsterMaterialData> materials)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int materialID = RNG.RandiRange(0, materials.Count - 1);
		return materials[materialID]; //!
	}

	private int GetLootValue(List<MonsterMaterialData> materials)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int materialRarityTotal = GetMaterialRarityTotal(materials);
		return RNG.RandiRange(0, materialRarityTotal);
	}

	private int GetMaterialRarityTotal(List<MonsterMaterialData> materials)
	{
		int total = 0;
		for (int i = 0; i < materials.Count; i++)
		{
			total += (int) materials[i].Rarity;
		}
		return total;
	}

	private void AddMaterials(List<MonsterMaterialData> monsterMaterials)
	{
		// Finds the items that are distinct
		IEnumerable<MonsterMaterialData> distinctMaterials = monsterMaterials.Distinct();

		// Find the count of each material in the list and add a collection log
		foreach (MonsterMaterialData distinctMaterial in distinctMaterials)
		{
			int distinctMaterialCount = monsterMaterials.FindAll(material => material == distinctMaterial).Count;
			distinctMaterial.Quantity += distinctMaterialCount;
			CollectionLogManager.Instance.AddLog(distinctMaterial, distinctMaterialCount);
		}
	}
}
