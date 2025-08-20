using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class Monster : Node
{
   public Monster() { }

   public Monster(GC.Dictionary<string, Variant> monsterDictionary)
   {
      Name = monsterDictionary["Name"].As<string>();
      Description = monsterDictionary["Description"].As<string>();
      Health = monsterDictionary["Health"].As<int>();

      List<string> elements = monsterDictionary["Elements"].As<GC.Array<string>>().ToList();
      foreach (string element in elements)
      {
         Elements.Add(Enum.Parse<ElementType>(element));
      }

      List<string> abnormalStatuses = monsterDictionary["AbnormalStats"].As<GC.Array<string>>().ToList();
      foreach (string abnormalStatus in abnormalStatuses)
      {
         AbnormalStats.Add(Enum.Parse<AbnormalStatType>(abnormalStatus));
      }

      List<string> elementWeaknesses = monsterDictionary["ElementalWeaknesses"].As<GC.Array<string>>().ToList();
      foreach (string elementWeakness in elementWeaknesses)
      {
         ElementalWeaknesses.Add(Enum.Parse<ElementType>(elementWeakness));
      }

      List<string> abnormalStatusWeaknesses = monsterDictionary["AbnormalStatWeaknesses"].As<GC.Array<string>>().ToList();
      foreach (string abnormalStatusWeakness in abnormalStatusWeaknesses)
      {
         AbnormalStatWeaknesses.Add(Enum.Parse<AbnormalStatType>(abnormalStatusWeakness));
      }

      List<string> locales = monsterDictionary["Locales"].As<GC.Array<string>>().ToList();
      foreach (string locale in locales)
      {
         Locales.Add(Enum.Parse<LocaleType>(locale));
      }
   }

   public new string Name;
   public string Description;
   public int Health;
   public List<ElementType> Elements = new List<ElementType>();
   public List<AbnormalStatType> AbnormalStats = new List<AbnormalStatType>();
   public List<ElementType> ElementalWeaknesses = new List<ElementType>();
   public List<AbnormalStatType> AbnormalStatWeaknesses = new List<AbnormalStatType>();

   public int Level = 1;
   public List<LocaleType> Locales = new List<LocaleType>();

   public void Clone(Monster monster, int level = 1)
   {
      Name = monster.Name;
      Description = monster.Description;
      Health = monster.Health;

      Elements.AddRange(monster.Elements);
      AbnormalStats.AddRange(monster.AbnormalStats);
      ElementalWeaknesses.AddRange(monster.ElementalWeaknesses);
      AbnormalStatWeaknesses.AddRange(monster.AbnormalStatWeaknesses);

      Level = level;
      Locales.AddRange(monster.Locales);
   }
}