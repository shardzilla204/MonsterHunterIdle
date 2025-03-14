using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class ItemBox : Node
{
   public List<ItemBoxMaterial> Materials = new List<ItemBoxMaterial>();

   public void AddMaterial(Material material, int quantity)
   {
      ItemBoxMaterial itemBoxMaterial = Materials.Find(itemBoxMaterial => itemBoxMaterial.Name == material.Name);
      itemBoxMaterial.Quantity += quantity;
   }
}