using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class MonsterLocaleFileLoader : Node
{
   public MonsterLocaleFileLoader()
   {
      LoadMonsterLocaleFile();
   }

   private GC.Dictionary<string, Variant> _monstersDictionary = new GC.Dictionary<string, Variant>();

   private void LoadMonsterLocaleFile()
   {
      string fileDirectory = "res://JSON/";
      string fileName = "MonsterLocale";
      string fileExtension = ".json";

      string filePath = $"{fileDirectory}{fileName}{fileExtension}";

      Json json = new Json();

      using FileAccess fileAccess = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
      string jsonString = fileAccess.GetAsText();

      if (json.Parse(jsonString) != Error.Ok) return;

      string loadSuccessMessage = "Monster Locale File Successfully Loaded";
      PrintRich.Print(TextColor.Green, loadSuccessMessage);

      GC.Dictionary<string, Variant> monsterMaterialsDictionary = (GC.Dictionary<string, Variant>) json.Data;
      _monstersDictionary = monsterMaterialsDictionary;
   }

   public GC.Dictionary<string, Variant> GetDictionary()
   {
      return _monstersDictionary;
   }
}