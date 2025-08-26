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
      Level = monsterDictionary["Level"].As<int>();

      List<string> specials = monsterDictionary["Specials"].As<GC.Array<string>>().ToList();
      foreach (string special in specials)
      {
         Specials.Add(Enum.Parse<SpecialType>(special));
      }

      List<string> specialWeaknesses = monsterDictionary["SpecialWeaknesses"].As<GC.Array<string>>().ToList();
      foreach (string specialWeakness in specialWeaknesses)
      {
         SpecialWeaknesses.Add(Enum.Parse<SpecialType>(specialWeakness));
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
   public List<SpecialType> Specials = new List<SpecialType>();
   public List<SpecialType> SpecialWeaknesses = new List<SpecialType>();

   public int Level = 1;
   public List<LocaleType> Locales = new List<LocaleType>();

   public void Clone(Monster monster, int level = 1)
   {
      Name = monster.Name;
      Description = monster.Description;
      Health = monster.Health;

      Specials.AddRange(monster.Specials);
      SpecialWeaknesses.AddRange(monster.SpecialWeaknesses);

      Level = Math.Max(monster.Level, level);
      Locales.AddRange(monster.Locales);
   }
}