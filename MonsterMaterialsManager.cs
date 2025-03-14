using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public enum MonsterSpecies
{
   GreatJagras,
   KuluYaKu,
   PukeiPukei
}

public partial class MonsterMaterialsManager : Node
{
   public MonsterMaterialsManager()
   {
      Materials = GetMonsterMaterials();
   }

   private MonsterMaterialsFileLoader _monsterMaterialsFileLoader = new MonsterMaterialsFileLoader();

   public List<MonsterMaterial> Materials = new List<MonsterMaterial>();

   public GC.Dictionary<string, Variant> GetMaterialDictionaries()
   {
      return _monsterMaterialsFileLoader.GetDictionary();
   }

   public GC.Dictionary<string, Variant> GetMaterialsDictionary(Monster monster)
   {
      GC.Dictionary<string, Variant> materialsDictionary = new GC.Dictionary<string, Variant>();

      GC.Dictionary<string, Variant> materialDictionaries = GetMaterialDictionaries();
      List<string> materialNames = materialDictionaries.Keys.ToList();
      foreach (string materialName in materialNames)
      {
         string monsterName = materialDictionaries["Monster"].As<string>();
         if (monsterName == monster.Name)
         {
            materialsDictionary.Add(materialName, materialDictionaries["materialName"]);
         }
      }
      AddWyvernGemShard(monster, materialsDictionary);

      return materialsDictionary;
   }

   private void AddWyvernGemShard(Monster monster, GC.Dictionary<string, Variant> materialsDictionary)
   {
      string wyvernGemShardName = "Wyvern Gem Shard";
      GC.Dictionary<string, Variant> wyvernGemShardDictionary = GetMaterialDictionaries()[wyvernGemShardName].As<GC.Dictionary<string, Variant>>();
      List<string> monsterNames = wyvernGemShardDictionary["Monster"].As<GC.Array<string>>().ToList();
      foreach (string monsterName in monsterNames)
      {
         if (monsterName == monster.Name) materialsDictionary.Add(wyvernGemShardName, GetMaterialDictionaries()[wyvernGemShardName]);
      }
   }

   private List<MonsterMaterial> GetMonsterMaterials()
   {
      List<MonsterMaterial> monsterMaterials = new List<MonsterMaterial>();
      List<string> materialNames = GetMaterialDictionaries().Keys.ToList();
      foreach (string materialName in materialNames)
      {
         MonsterMaterial material = GetMonsterMaterial(materialName);
         monsterMaterials.Add(material);
      }
      return monsterMaterials;
   }

   private MonsterMaterial GetMonsterMaterial(string materialName)
   {
      GC.Dictionary<string, Variant> materialDictionary = GetMaterialDictionaries()[materialName].As<GC.Dictionary<string, Variant>>();
      MonsterMaterial monsterMaterial = new MonsterMaterial(materialName, materialDictionary);
      return monsterMaterial;
   }

   public List<ItemBoxMaterial> GetItemBoxMaterials()
   {
      List<ItemBoxMaterial> itemBoxMaterials = new List<ItemBoxMaterial>();
      List<string> materialNames = GetMaterialDictionaries().Keys.ToList();
      foreach (string materialName in materialNames)
      {
         GC.Dictionary<string, Variant> monsterMaterialDictionary = GetMaterialDictionaries()[materialName].As<GC.Dictionary<string, Variant>>();
         ItemBoxMaterial inventoryMaterial = new ItemBoxMaterial(materialName, monsterMaterialDictionary);
         itemBoxMaterials.Add(inventoryMaterial);
      }
      return itemBoxMaterials;
   }
}