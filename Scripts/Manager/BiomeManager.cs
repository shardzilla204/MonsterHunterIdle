using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public enum BiomeType
{
	None = -1,
	Forest,
	Desert,
	Swamp
}

public partial class BiomeManager : Node
{
	public Biome Biome;
	public List<Biome> Biomes = new List<Biome>();

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
		Biome = Biomes.Find(biome => biome.Type == BiomesQueue[0]);

		clockwise = (BiomesQueue.Count - 1, 0);
		counterClockwise = (0, BiomesQueue.Count - 1);

		SetBiomes();
   }

	public void SetBiomes()
	{
		int maxBiomeCount = 3;
		for (int i = 0; i < maxBiomeCount; i++)
		{
			BiomeType biomeType = (BiomeType) i;
			Biome biome = new Biome(biomeType);
			Biomes.Add(biome);
		}
	}

	public void CycleBiome(bool isClockwise)
	{
		int listPosition = isClockwise ? clockwise.listPosition : counterClockwise.listPosition;
		int position = isClockwise ? clockwise.position : counterClockwise.position;

		BiomeType biomeType = BiomesQueue[listPosition];
		BiomesQueue.Remove(biomeType);
		BiomesQueue.Insert(position, biomeType);

		Biome = Biomes.Find(biome => biome.Type == BiomesQueue[0]);
	}

	public BiomeMaterial GetBiomeMaterial()
	{
		int materialRarity = GetRandomMaterialRarity();
		BiomeMaterial biomeMaterial = GetRandomMaterial();

		while (materialRarity > 0)
		{
			materialRarity -= biomeMaterial.Rarity;
			biomeMaterial = GetRandomMaterial();
		}

		return biomeMaterial;
	}

	private BiomeMaterial GetRandomMaterial()
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		List<BiomeMaterial> biomeMaterials = MonsterHunterIdle.MaterialManager.FindBiomeMaterials(Biome.Type);
		int randomValue = RNG.RandiRange(0, biomeMaterials.Count - 1);

		return biomeMaterials[randomValue];
	}

	private int GetRandomMaterialRarity()
	{
		int materialRarityTotal = GetMaterialRarityTotal();
		RandomNumberGenerator RNG = new RandomNumberGenerator();

		return RNG.RandiRange(0, materialRarityTotal);
	}

	private int GetMaterialRarityTotal()
	{
		int materialRarityTotal = 0;
		List<BiomeMaterial> biomeMaterials = MonsterHunterIdle.MaterialManager.FindBiomeMaterials(Biome.Type);

		for (int i = 0; i < biomeMaterials.Count; i++)
		{
			materialRarityTotal += biomeMaterials[i].Rarity;
		}
		return materialRarityTotal;
	}

	public Texture2D GetLocaleIcon(BiomeType biomeType)
	{
		string fileDirectory = "res://Assets/Images/Biome/LocaleIcon/";
		string fileName = $"{biomeType}LocaleIcon";
		string fileExtension = ".png";

		string filePath = $"{fileDirectory}{fileName}{fileExtension}";
		return ResourceLoader.Load<Texture2D>(filePath);
	}

	public Texture2D GetBackground(BiomeType biomeType)
	{
		string fileDirectory = "res://Assets/Images/Biome/";
		string fileName = $"{biomeType}";
		string fileExtension = ".png";

		string filePath = $"{fileDirectory}{fileName}{fileExtension}";
		return ResourceLoader.Load<Texture2D>(filePath);
	}

	public Texture2D GetGatherIcon(BiomeType biomeType)
	{
		string fileDirectory = "res://Assets/Images/Biome/GatherIcon/";
		string fileName = $"{GetBiomeMaterialString(biomeType)}GatherIcon";
		string fileExtension = ".png";

		string filePath = $"{fileDirectory}{fileName}{fileExtension}";
		return ResourceLoader.Load<Texture2D>(filePath);
	}

	private string GetBiomeMaterialString(BiomeType biomeType) => biomeType switch 
	{
		BiomeType.Forest => "Vegetation",
		BiomeType.Desert => "Desert",
		BiomeType.Swamp => "Ore",
		_ => "Vegetation"
	};
}
