using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public enum LocaleType
{
	None = -1,
	Forest,
	Desert,
	Swamp
}

public partial class LocaleManager : Node
{
	[Export]
	private LocaleType _startingLocale = LocaleType.Forest;

	public static Locale Locale;
	public static List<Locale> LocaleQueue = new List<Locale>();

	public static List<LocaleMaterial> Materials = new List<LocaleMaterial>();

	public override void _EnterTree()
	{
		LoadMaterials();
		SetLocaleQueue();

		Locale = LocaleQueue.Find(locale => locale.Type == _startingLocale); // ! Call this after SetLocaleQueue()
	}

	private static void LoadMaterials()
	{
		string fileName = "LocaleMaterials";
		GC.Dictionary<string, Variant> localeMaterialData = MonsterHunterIdle.LoadFile(fileName).As<GC.Dictionary<string, Variant>>();
		if (localeMaterialData == null) return;

		List<GC.Dictionary<string, Variant>> localeMaterialDictionaires = localeMaterialData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
		
		// Set locale materials
		foreach (GC.Dictionary<string, Variant> dictionary in localeMaterialDictionaires)
		{
			LocaleMaterial localeMaterial = new LocaleMaterial(dictionary);
			Materials.Add(localeMaterial);
		}

		PrintRich.PrintLocales(TextColor.Blue);
	}

	private static void SetLocaleQueue()
	{
		List<LocaleType> localeQueue = new List<LocaleType>() { LocaleType.Forest, LocaleType.Desert, LocaleType.Swamp };
		foreach (LocaleType localeType in localeQueue)
		{
			Locale locale = new Locale(localeType);
			LocaleQueue.Add(locale);
		}
	}

	public static Task<bool> CycleLocale(bool isGettingNext)
	{
		try
		{
			Locale targetLocale = isGettingNext ? GetNextLocale() : GetPreviousLocale();
			Locale currentLocale = Locale;

			// Grab the current element. Remove and append to the end of the list.
			LocaleQueue.Remove(Locale);
			LocaleQueue.Add(Locale);

			// If getting the previous element, grab the last element and move it to the front of the list.
			if (!isGettingNext)
			{
				LocaleQueue.Remove(targetLocale);
				LocaleQueue.Insert(0, targetLocale); // Append to the front of the list
			}

			Locale = LocaleQueue[0]; // Set the new locale
			
			// Console message
			string localeChangedMessage = $"Locale Changed To {Locale.Name}";
			PrintRich.PrintLine(TextColor.Orange, localeChangedMessage);

			return Task.FromResult(true);
		}
		catch (IndexOutOfRangeException)
		{
			return Task.FromResult(false);
		}
	}

	public static Locale GetNextLocale()
	{
		List<Locale> locales = LocaleQueue;
		LocaleType localeType = Locale.Type;
		return locales.SkipWhile(locale => locale.Type != localeType).Skip(1).DefaultIfEmpty(locales[0]).FirstOrDefault();
	}

	public static Locale GetPreviousLocale()
	{
		List<Locale> locales = LocaleQueue;
		LocaleType localeType = Locale.Type;
		return locales.TakeWhile(locale => locale.Type != localeType).DefaultIfEmpty(locales[locales.Count - 1]).LastOrDefault();
	}

	public static LocaleMaterial GetLocaleMaterial()
	{
		List<LocaleMaterial> localeMaterials = FindMaterials(Locale.Type);

		int total = 0;
		foreach (LocaleMaterial material in localeMaterials)
		{
			int materialWeight = MonsterHunterIdle.GetMaterialWeight(material.Rarity);
			total += materialWeight;
		}
		
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int randomNumber = RNG.RandiRange(0, total);
		foreach (LocaleMaterial material in localeMaterials)
		{
			int materialWeight = MonsterHunterIdle.GetMaterialWeight(material.Rarity);
			if (randomNumber <= materialWeight)
			{
				return material;
			}
			else
			{
				randomNumber -= materialWeight;
			}
		}

		return null;
	}

	public static LocaleMaterial GetLocaleMaterial(LocaleType localeType, int rarity)
	{
		List<LocaleMaterial> localeMaterials = FindMaterials(localeType);
		return localeMaterials.Find(material => material.Rarity == rarity && material.Locales.Count == 1 && material.Locales.Contains(localeType));
	}

	public static LocaleMaterial FindMaterial(string materialName)
	{
		return Materials.Find(material => material.Name == materialName);
	}

	private static List<LocaleMaterial> FindMaterials(LocaleType localeType)
	{
		List<LocaleMaterial> localeMaterials = Materials.FindAll(material => material.Locales.Contains(localeType));
		return localeMaterials;
	}

	public static Texture2D GetBackground(LocaleType localeType)
	{
		string filePath = $"res://Assets/Images/Locale/{localeType}.png";
		return ResourceLoader.Load<Texture2D>(filePath);
	}
	
	public static Texture2D GetLocaleIcon(LocaleType localeType)
	{
		string filePath = $"res://Assets/Images/Locale/Locale/{localeType}LocaleIcon.png";
		return ResourceLoader.Load<Texture2D>(filePath);
	}

	public static Texture2D GetGatherIcon(LocaleType localeType)
	{
		string filePath = $"res://Assets/Images/Locale/Gather/{GetLocaleMaterialString(localeType)}GatherIcon.png";
		return ResourceLoader.Load<Texture2D>(filePath);
	}

	private static string GetLocaleMaterialString(LocaleType localeType) => localeType switch 
	{
		LocaleType.Forest => "Vegetation",
		LocaleType.Desert => "Bone",
		LocaleType.Swamp => "Ore",
		_ => "Vegetation"
	};
}
