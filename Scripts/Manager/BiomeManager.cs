using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public enum BiomeType
{
	Forest,
	Desert,
	Swamp
}

public partial class BiomeManager : Node
{
	[Signal]
	public delegate void UpdatedEventHandler();

	private static BiomeManager _instance;
	public static BiomeManager Instance
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
				GD.PrintRich($"{nameof(BiomeManager)} already exists");
			}
		}
	}

	[Export]
	private ForestData _forestData;

	[Export]
	private DesertData _desertData;

	[Export]
	private SwampData _swampData;

	public BiomeData Biome;

	public Dictionary<BiomeType, BiomeData> Biomes = new Dictionary<BiomeType, BiomeData>();
	public List<BiomeType> BiomesQueue = new List<BiomeType>()
	{
		BiomeType.Forest,
		BiomeType.Desert,
		BiomeType.Swamp
	};
	private (int listPosition, int position) clockwise;
	private (int listPosition, int position) counterClockwise;

    public override void _Ready()
    {
        Instance = this;

		Biomes = new Dictionary<BiomeType, BiomeData>()
		{
			{ BiomeType.Forest, _forestData }, 
			{ BiomeType.Desert, _desertData }, 
			{ BiomeType.Swamp, _swampData }
		};
		
		Biome = Biomes[BiomesQueue[0]];

		clockwise = (BiomesQueue.Count - 1, 0);
		counterClockwise = (0, BiomesQueue.Count - 1);
    }

	public void CycleBiome(bool isClockwise)
	{
		int listPosition = isClockwise ? clockwise.listPosition : counterClockwise.listPosition;
		int position = isClockwise ? clockwise.position : counterClockwise.position;

		BiomeType biomeType = BiomesQueue[listPosition];
		BiomesQueue.Remove(biomeType);
		BiomesQueue.Insert(position, biomeType);

		Biome = Biomes[BiomesQueue[0]];
		EmitSignal(SignalName.Updated);
	}

	public dynamic GetBiomeMaterial()
	{
		dynamic biomeData = GetBiomeData();
		int lootValue = GetRandomLootValue(biomeData);
		dynamic biomeMaterial = GetRandomMaterial(biomeData);

		while (lootValue > 0)
		{
			lootValue -= biomeMaterial.Rarity;
			biomeMaterial = GetRandomMaterial(biomeData);
		}

		return biomeMaterial;
	}

	private dynamic GetBiomeData()
	{
		BiomeData _biomeData = Biome;
		if (_biomeData is ForestData forestData)
		{
			_biomeData = forestData;
		}
		else if (_biomeData is DesertData desertData)
		{
			_biomeData = desertData;
		}
		else if (_biomeData is SwampData swampData)
		{
			_biomeData = swampData;
		}
		return _biomeData;
	}	

	private dynamic GetRandomMaterial(dynamic biomeData)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int materialID = RNG.RandiRange(0, biomeData.Materials.Count - 1);
		return biomeData.Materials[materialID];
	}

	private int GetRandomLootValue(dynamic biomeData)
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int materialRarityTotal = GetBiomeMaterialRarityTotal(biomeData);
		return RNG.RandiRange(0, materialRarityTotal);
	}

	private int GetBiomeMaterialRarityTotal(dynamic biomeData)
	{
		int total = 0;
		for (int i = 0; i < biomeData.Materials.Count; i++)
		{
			total += (int) biomeData.Materials[i].Rarity;
		}
		return total;
	}
}
