using Godot;
using Godot.Collections;
using System;

namespace MonsterHunterIdle;

public partial class BiomeMaterial : Node
{
   public BiomeMaterial(string biomeMaterialName, Dictionary<string, Variant> biomeMaterialDictionary)
   {
      Name = biomeMaterialName;
      Biome = Enum.Parse<BiomeType>(biomeMaterialDictionary["Biome"].As<string>());
      Description = biomeMaterialDictionary["Description"].As<string>();
      Rarity = biomeMaterialDictionary["Rarity"].As<int>();

      string materialColorString = biomeMaterialDictionary["Color"].As<string>();
      MaterialColor materialColor = MonsterHunterIdle.MaterialManager.GetMaterialColor(materialColorString);
      Color = materialColor;

      string materialTypeString = biomeMaterialDictionary["Type"].As<string>();
      MaterialType materialType = MonsterHunterIdle.MaterialManager.GetMaterialType(materialTypeString);
      Type = materialType;
   }

   public new string Name;
   public BiomeType Biome;
   public string Description;
   public int Rarity;
   public MaterialColor Color;
   public MaterialType Type;
}