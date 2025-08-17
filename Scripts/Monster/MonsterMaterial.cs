using System.Collections.Generic;
using System.Linq;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class MonsterMaterial : Material
{
   public MonsterMaterial(GC.Dictionary<string, Variant> dictionary) : base(dictionary)
   {
      Monsters = dictionary["Monsters"].As<GC.Array<string>>().ToList();
   }

   public List<string> Monsters = new List<string>();
}