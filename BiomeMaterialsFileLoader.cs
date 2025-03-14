using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class BiomeMaterialsFileLoader
{
   public BiomeMaterialsFileLoader()
   {
      LoadBiomeMaterialsFile();
   }

   private GC.Dictionary<string, Variant> _biomeMaterialsDictionary = new GC.Dictionary<string, Variant>();

   private void LoadBiomeMaterialsFile()
   {
      string fileDirectory = "res://JSON/";
      string fileName = "BiomeMaterials";
      string fileExtension = ".json";

      string filePath = $"{fileDirectory}{fileName}{fileExtension}";

      Json json = new Json();

      using FileAccess fileAccess = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
      string jsonString = fileAccess.GetAsText();
      
      if (json.Parse(jsonString) != Error.Ok) return;

      string loadSuccessMessage = "Biome Materials File Successfully Loaded";
      PrintRich.Print(TextColor.Green, loadSuccessMessage);

      GC.Dictionary<string, Variant> biomeMaterialsDictionary = (GC.Dictionary<string, Variant>) json.Data;
      _biomeMaterialsDictionary = biomeMaterialsDictionary;
   }

   public GC.Dictionary<string, Variant> GetDictionary()
   {
      return _biomeMaterialsDictionary;
   }
}