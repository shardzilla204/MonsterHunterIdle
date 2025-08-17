using Godot;
using GC = Godot.Collections;
using System;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class LocaleMaterial : Material
{
   public LocaleMaterial(GC.Dictionary<string, Variant> dictionary) : base (dictionary)
   {
      GC.Array<string> localeNames = dictionary["Locales"].As<GC.Array<string>>();
      foreach (string localeName in localeNames)
      {
         LocaleType localeType = Enum.Parse<LocaleType>(localeName);
         Locales.Add(localeType);
      }
   }

   public List<LocaleType> Locales = new List<LocaleType>();
}