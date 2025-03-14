using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class ItemBoxMaterial : Material
{
   public ItemBoxMaterial(string materialName, Dictionary<string, Variant> dictionary) : base(materialName, dictionary)
   {
      Name = materialName;
      Description = dictionary["Description"].As<string>();
      Rarity = dictionary["Rarity"].As<int>();
      Color = MonsterHunterIdle.MaterialManager.GetMaterialColor(dictionary["Color"].As<string>());
      Type = MonsterHunterIdle.MaterialManager.GetMaterialType(dictionary["Type"].As<string>());
   }

   public BiomeType BiomeType;
   public MonsterSpecies MonsterSpecies;
   public int Quantity = 0;
}