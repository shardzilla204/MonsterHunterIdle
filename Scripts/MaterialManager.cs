using System;
using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public enum MaterialColor
{
   Yellow,
   DeepGreen,
   DarkRed,
   Red,
   Blue,
   LightBlue,
   Gray,
   White,
   Orange,
   Purple,
   Tan
}

public enum MaterialType
{
   Scale,
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
   Ore
}

public partial class MaterialManager : Node
{
   public MaterialManager()
   {
      MonsterMaterialsManager monsterMaterialsManager = new MonsterMaterialsManager();
      MonsterMaterials = monsterMaterialsManager.Materials;

      BiomeMaterialsManager biomeMaterialsManager = new BiomeMaterialsManager();
      BiomeMaterials = biomeMaterialsManager.Materials;
   }
   
   public List<BiomeMaterial> BiomeMaterials = new List<BiomeMaterial>();
   public List<MonsterMaterial> MonsterMaterials = new List<MonsterMaterial>();

   public int GetMaterialRarityValue(int rarity) => rarity switch
   {
      1 => 2500,
      2 => 2000,
      3 => 1500,
      4 => 1000,
      5 => 500,
      6 => 250,
      7 => 200,
      8 => 150,
      9 => 100,
      10 => 50,
      _ => throw new ArgumentOutOfRangeException("Rarity", $"Not expected rarity value: {rarity}")
   };

   public MaterialType GetMaterialType(string typeName)
   {
      typeName = typeName.Replace(" ", "");
      return Enum.Parse<MaterialType>(typeName);
   }

   public MaterialColor GetMaterialColor(string colorName)
   {
      colorName = colorName.Replace(" ", "");
      return Enum.Parse<MaterialColor>(colorName);
   }

   public List<BiomeMaterial> FindBiomeMaterials(BiomeType biomeType)
   {
      List<BiomeMaterial> biomeMaterials = BiomeMaterials.FindAll(material => material.Biome == biomeType);
      return biomeMaterials;
   }

   public List<MonsterMaterial> FindMonsterMaterials(Monster monster)
   {
      MonsterSpecies monsterSpecies = ChangeMonsterNameToSpecies(monster);
      List<MonsterMaterial> monsterMaterials = MonsterMaterials.FindAll(material => material.MonsterSpecies == monsterSpecies);
      return monsterMaterials;
   }

   private MonsterSpecies ChangeMonsterNameToSpecies(Monster monster)
   {
      string monsterName = monster.Name;
      monsterName = monsterName.Replace(" ", "");
      monsterName = monsterName.Replace("-", "");

      return Enum.Parse<MonsterSpecies>(monsterName);
   }
}