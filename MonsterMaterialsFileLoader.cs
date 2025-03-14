using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class MonsterMaterialsFileLoader : Node
{
   public MonsterMaterialsFileLoader()
   {
      LoadMonsterMaterialsFile();
   }

   private GC.Dictionary<string, Variant> _monsterMaterialsDictionary = new GC.Dictionary<string, Variant>();

   private void LoadMonsterMaterialsFile()
   {
      string fileDirectory = "res://JSON/";
      string fileName = "MonsterMaterials";
      string fileExtension = ".json";

      string filePath = $"{fileDirectory}{fileName}{fileExtension}";

      Json json = new Json();

      using FileAccess fileAccess = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
      string jsonString = fileAccess.GetAsText();

      if (json.Parse(jsonString) != Error.Ok) return;

      string loadSuccessMessage = "Monster Materials File Successfully Loaded";
      PrintRich.Print(TextColor.Green, loadSuccessMessage);

      GC.Dictionary<string, Variant> monsterMaterialsDictionary = (GC.Dictionary<string, Variant>) json.Data;
      _monsterMaterialsDictionary = monsterMaterialsDictionary;
   }

   public GC.Dictionary<string, Variant> GetDictionary()
   {
      return _monsterMaterialsDictionary;
   }
}