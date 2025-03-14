using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class BiomeMaterialsManager : Node
{
   public BiomeMaterialsManager()
   {
      Materials = GetBiomeMaterials();
   }

   private BiomeMaterialsFileLoader _biomeMaterialsFileLoader = new BiomeMaterialsFileLoader();
   public List<BiomeMaterial> Materials = new List<BiomeMaterial>();

   public GC.Dictionary<string, Variant> GetBiomeMaterialsDictionary()
   {
      return _biomeMaterialsFileLoader.GetDictionary();
   }

   private List<BiomeMaterial> GetBiomeMaterials()
   {
      List<BiomeMaterial> biomeMaterials = new List<BiomeMaterial>();
      List<string> biomeMaterialNames = GetBiomeMaterialsDictionary().Keys.ToList();
      foreach (string biomeMaterialName in biomeMaterialNames)
      {
         BiomeMaterial biomeMaterial = GetBiomeMaterial(biomeMaterialName);
         biomeMaterials.Add(biomeMaterial);
      }
      return biomeMaterials;
   }

   private BiomeMaterial GetBiomeMaterial(string biomeMaterialName)
   {
      GC.Dictionary<string, Variant> biomeMaterialDictionary = GetBiomeMaterialsDictionary()[biomeMaterialName].As<GC.Dictionary<string, Variant>>();
      BiomeMaterial biomeMaterial = new BiomeMaterial(biomeMaterialName, biomeMaterialDictionary);
      return biomeMaterial;
   }

   public List<ItemBoxMaterial> GetItemBoxMaterials()
   {
      List<ItemBoxMaterial> itemBoxMaterials = new List<ItemBoxMaterial>();
      List<string> biomeMaterialNames = GetBiomeMaterialsDictionary().Keys.ToList();
      foreach (string biomeMaterialName in biomeMaterialNames)
      {
         GC.Dictionary<string, Variant> materialDictionary = GetBiomeMaterialsDictionary()[biomeMaterialName].As<GC.Dictionary<string, Variant>>();
         ItemBoxMaterial inventoryMaterial = new ItemBoxMaterial(biomeMaterialName, materialDictionary);
         itemBoxMaterials.Add(inventoryMaterial);
      }
      return itemBoxMaterials;
   }
}
