using Godot;
using Godot.Collections;
using System;

namespace MonsterHunterIdle;

public partial class MonsterMaterial : Node
{
   public MonsterMaterial(string monsterMaterialName, Dictionary<string, Variant> monsterMaterialDictionary)
   {
      Name = monsterMaterialName;
      MonsterSpecies = Enum.Parse<MonsterSpecies>(monsterMaterialDictionary["Monster"].As<string>());
      Description = monsterMaterialDictionary["Description"].As<string>();
      Rarity = monsterMaterialDictionary["Rarity"].As<int>();

      string materialColorString = monsterMaterialDictionary["Color"].As<string>();
      MaterialColor materialColor = MonsterHunterIdle.MaterialManager.GetMaterialColor(materialColorString);
      Color = materialColor;

      string materialTypeString = monsterMaterialDictionary["Type"].As<string>();
      MaterialType materialType = MonsterHunterIdle.MaterialManager.GetMaterialType(materialTypeString);
      Type = materialType;
   }

   public new string Name;
   public MonsterSpecies MonsterSpecies;
   public string Description;
   public int Rarity;
   public MaterialColor Color;
   public MaterialType Type;
}