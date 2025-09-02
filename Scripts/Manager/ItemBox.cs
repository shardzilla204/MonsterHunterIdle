using System.Collections.Generic;
using System.Linq;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class ItemBox : Node
{
   [Export]
   private bool _hasStartingMaterials = false;

   [Export]
   private GC.Array<GC.Dictionary<string, Variant>> _startingMaterials = new GC.Array<GC.Dictionary<string, Variant>>()
   {
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Iron Ore" },
         { "Amount", 500 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Machalite Ore" },
         { "Amount", 500 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Dragonite Ore" },
         { "Amount", 500 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Earth Crystal" },
         { "Amount", 500 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Godbug" },
         { "Amount", 500 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Sharp Claw" },
         { "Amount", 500 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Great Jagras Scale" },
         { "Amount", 100 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Great Jagras Hide" },
         { "Amount", 100 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Great Jagras Claw" },
         { "Amount", 100 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Great Jagras Mane" },
         { "Amount", 100 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Name", "Great Jagras Primescale" },
         { "Amount", 100 }
      },
   };

   private static bool _HasStartingMaterials = false;
   private static GC.Array<GC.Dictionary<string, Variant>> _StartingMaterials = new GC.Array<GC.Dictionary<string, Variant>>();

   public static List<Material> Materials = new List<Material>();

   public override void _EnterTree()
   {
      MonsterHunterIdle.Signals.LocaleMaterialAdded += Materials.Add;
      MonsterHunterIdle.Signals.MonsterMaterialAdded += Materials.Add;

      _StartingMaterials = _startingMaterials;
   }

   private static void AddStartingMaterials()
   {
      foreach (GC.Dictionary<string, Variant> materialDictionary in _StartingMaterials)
      {
         string materialName = materialDictionary["Name"].As<string>();
         Material material = MonsterHunterIdle.FindMaterial(materialName);

         int materialAmount = materialDictionary["Amount"].As<int>();
         for (int i = 0; i < materialAmount; i++)
         {
            Materials.Add(material);
         }
      }
   }

   public static Material FindMaterial(string materialName)
   {
      return Materials.Find(material => material.Name == materialName);
   }

   public static List<Material> FindAllMaterial(string materialName)
   {
      return Materials.FindAll(material => material.Name == materialName);
   }

   public static void SubtractMaterial(Material material, int amount)
   {
      for (int i = 0; i < amount; i++)
      {
         Materials.Remove(material);
      }
   }

   public static int GetSellValue(Material material) => material.Rarity switch
   {
      1 => 1,
      2 => 5,
      3 => 10,
      4 => 15,
      5 => 30,
      6 => 50,
      _ => 1
   };

   // Data methods
   /// <see cref="GameManager.SaveGame">
   public static GC.Dictionary<string, Variant> GetData()
   {
      // GC.Array<string> materialsData = new GC.Array<string>();
      GC.Dictionary<string, Variant> materialsData = new GC.Dictionary<string, Variant>();

      IEnumerable<Material> distinctMaterials = Materials.Distinct();

      foreach (Material material in distinctMaterials)
      {
         int materialCount = FindAllMaterial(material.Name).Count;
         materialsData.Add(material.Name, materialCount);
      }
      return materialsData;
   }

   /// <see cref="GameManager.LoadGame">
   public static void SetData(GC.Dictionary<string, Variant> itemBoxData)
   {
      foreach (string materialName in itemBoxData.Keys)
      {
         Material material = MonsterHunterIdle.FindMaterial(materialName);
         int materialCount = itemBoxData[materialName].As<int>();
         for (int i = 0; i < materialCount; i++)
         {
            Materials.Add(material);
         }
      }
   }

   /// <see cref="GameManager.DeleteGame">
   public static void DeleteData()
   {
      Materials.Clear();

      if (_HasStartingMaterials) AddStartingMaterials();
   }
}