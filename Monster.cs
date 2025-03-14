using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class Monster : Node
{
   public Monster(string monsterName, GC.Dictionary<string, Variant> monsterDictionary)
   {
      Name = monsterName;
      Description = monsterDictionary["Description"].As<string>();
      BaseHealth = monsterDictionary["BaseHealth"].As<int>();
      List<string> elementStrings = monsterDictionary["Elements"].As<GC.Array<string>>().ToList();
      foreach (string elementString in elementStrings)
      {
         Elements.Add(Enum.Parse<ElementType>(elementString));
      }
      List<string> abnormalStatusStrings = monsterDictionary["AbnormalStatuses"].As<GC.Array<string>>().ToList();
      foreach (string abnormalStatusString in abnormalStatusStrings)
      {
         AbnormalStatuses.Add(Enum.Parse<AbnormalStatusType>(abnormalStatusString));
      }
      List<string> elementWeaknessStrings = monsterDictionary["ElementWeaknesses"].As<GC.Array<string>>().ToList();
      foreach (string elementWeaknessString in elementWeaknessStrings)
      {
         ElementWeaknesses.Add(Enum.Parse<ElementType>(elementWeaknessString));
      }
      List<string> abnormalStatusWeaknessStrings = monsterDictionary["AbnormalStatusWeaknesses"].As<GC.Array<string>>().ToList();
      foreach (string abnormalStatusWeaknessString in abnormalStatusWeaknessStrings)
      {
         AbnormalStatusWeaknesses.Add(Enum.Parse<AbnormalStatusType>(abnormalStatusWeaknessString));
      }
   }

   public new string Name;
   public string Description;
   public int BaseHealth;
   public List<ElementType> Elements = new List<ElementType>();
   public List<AbnormalStatusType> AbnormalStatuses = new List<AbnormalStatusType>();
   public List<ElementType> ElementWeaknesses = new List<ElementType>();
   public List<AbnormalStatusType> AbnormalStatusWeaknesses = new List<AbnormalStatusType>();

   public int Level = 1;
   public List<MonsterMaterial> Materials = new List<MonsterMaterial>();
   public List<BiomeType> Locales = new List<BiomeType>();
}