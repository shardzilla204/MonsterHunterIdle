using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class MonstersFileLoader : Node
{
   public MonstersFileLoader()
   {
      LoadMonstersFile();
   }

   private GC.Dictionary<string, Variant> _monsterDictionaries = new GC.Dictionary<string, Variant>();

   private void LoadMonstersFile()
   {
      string fileDirectory = "res://JSON/";
      string fileName = "Monsters";
      string fileExtension = ".json";

      string filePath = $"{fileDirectory}{fileName}{fileExtension}";

      Json json = new Json();

      using FileAccess fileAccess = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
      string jsonString = fileAccess.GetAsText();

      if (json.Parse(jsonString) != Error.Ok) return;

      string loadSuccessMessage = "Monsters File Successfully Loaded";
      PrintRich.Print(TextColor.Green, loadSuccessMessage);

      GC.Dictionary<string, Variant> monsterDictionaries = (GC.Dictionary<string, Variant>) json.Data;
      _monsterDictionaries = monsterDictionaries;
   }

   public GC.Dictionary<string, Variant> GetDictionary()
   {
      return _monsterDictionaries;
   }
}