using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class Material : Node
{
   public Material(string materialName, Dictionary<string, Variant> dictionary)
   {
      Name = materialName;
      Description = dictionary["Description"].As<string>();
      Rarity = dictionary["Rarity"].As<int>();
      Color = MonsterHunterIdle.MaterialManager.GetMaterialColor(dictionary["Color"].As<string>());
      Type = MonsterHunterIdle.MaterialManager.GetMaterialType(dictionary["Type"].As<string>());
   }

   public new string Name;
   public string Description;
   public int Rarity = 1;
   public MaterialColor Color;
   public MaterialType Type;
}