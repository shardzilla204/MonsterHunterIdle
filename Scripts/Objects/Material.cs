using System;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public enum MaterialColor
{
   Yellow,
   Green,
   DeepGreen,
   DarkRed,
   Red,
   Blue,
   LightBlue,
   Grey,
   White,
   DeepOrange,
   Orange,
   Purple,
   Tan,
   Teal,
   Pink,
   Brown
}

public enum MaterialType
{
   Scale,
   Tail,
   Shell,
   Hide,
   Bug,
   Plant,
   Claw,
   Skull,
   Mushroom,
   Seed,
   Wing,
   MonsterPart,
   Gem,
   MonsterBone,
   Sac,
   Ore,
   Plate
}

public partial class Material : Node
{
   public Material(Dictionary<string, Variant> dictionary)
   {
      Name = dictionary["Name"].As<string>();

      Description = dictionary["Description"].As<string>();
      Rarity = dictionary["Rarity"].As<int>();

      string colorName = dictionary["Color"].As<string>();
      Color = Enum.Parse<MaterialColor>(colorName);

      string typeName = dictionary["Type"].As<string>();
      Type = Enum.Parse<MaterialType>(typeName);
   }

   public new string Name;
   public string Description;
   public int Rarity = 1;
   public MaterialColor Color;
   public MaterialType Type;
}