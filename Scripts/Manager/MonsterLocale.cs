using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class MonsterLocale : Node
{
   public List<Monster> Monsters = new List<Monster>();

   public void SetMonsters(Dictionary<string, Variant> monsterLocaleDictionary)
   {

   }
}