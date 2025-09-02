using System.Collections.Generic;
using System.Reflection;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class HunterManager : Node
{
   [Export]
   private int _startingZenny = 100;

   public static int StartingZenny;

   public static GC.Dictionary<string, int> MonstersSlayed = new GC.Dictionary<string, int>();

   public override void _EnterTree()
   {
      MonsterHunterIdle.Signals.MonsterSlayed += OnMonsterSlayed;

      StartingZenny = _startingZenny;
      Hunter.ResetData();

      FillMonstersSlayedDictionary();
   }

   // * START - Signal Methods
   public static void OnMonsterSlayed(Monster monster)
   {
      MonstersSlayed[monster.Name]++;
   }
   // * END - Signal Methods

   // Used for resetting
   private static void FillMonstersSlayedDictionary()
   {
      List<Monster> monsters = MonsterManager.Monsters;
      foreach (Monster monster in monsters)
      {
         MonstersSlayed.Add(monster.Name, 0);
      }
   }

   public static void AddHunterPoints(int progressAmount)
   {
      Hunter.Points += progressAmount;

      MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.HunterPointsChanged);

      CheckHunterProgress();
   }

   public static void AddZenny(int zennyAmount)
   {
      Hunter.Zenny += zennyAmount;
   }

   private static void CheckHunterProgress()
   {
      if (Hunter.Points < Hunter.PointsRequired || Hunter.Rank >= Hunter.MaxRank) return;

      Hunter.Rank++; // Increase Rank
      Hunter.Points = 0; // Reset Points
      IncreaseHunterProgress();

      MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.HunterLeveledUp);
   }

   private static void IncreaseHunterProgress()
   {
      int pointsIncrease = 100;
      Hunter.PointsRequired += pointsIncrease;
   }

   private static Armor FindArmor(ArmorCategory armorCategory)
   {
      return EquipmentManager.CraftedArmor.Find(armor => armor.Category == armorCategory);
   }

   public static Equipment FindWeapon(Weapon targetWeapon)
   {
      Equipment desiredWeapon = EquipmentManager.CraftedWeapons.Find(weapon => weapon.Category == targetWeapon.Category && weapon.Tree == targetWeapon.Tree);

      if (desiredWeapon == null)
      {
         string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
         string message = $"Couldn't Find Desired Weapon Of {targetWeapon.Category} {targetWeapon.Tree}";
         string result = $"Returning Null";
         PrintRich.PrintError(className, message, result);
      }

      return desiredWeapon;
   }

   public static Armor FindArmor(Armor targetArmor)
   {
      Armor desiredArmor = EquipmentManager.CraftedArmor.Find(armor => armor.Category == targetArmor.Category && armor.Set == targetArmor.Set);

      if (desiredArmor == null)
      {
         string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
         string message = $"Couldn't Find Desired Armor Of {targetArmor.Category} {targetArmor.Set}";
         string result = $"Returning Null";
         PrintRich.PrintError(className, message, result);
      }

      return desiredArmor;
   }

   public static bool IsEquipped(Equipment equipment)
   {
      bool isEquipped = false;
      if (equipment is Weapon weapon)
      {
         if (Hunter.Weapon.Name == "") return false;
         isEquipped = Hunter.Weapon.Tree == weapon.Tree;
      }
      else if (equipment is Armor armor)
      {
         Armor desiredArmor = GetArmor(armor.Category);
         if (desiredArmor.Name == "") return false;

         isEquipped = desiredArmor.Set == armor.Set;
      }
      return isEquipped;
   }

   private static Armor GetArmor(ArmorCategory targetCategory)
   {
      Armor armor = null;
      switch (targetCategory)
      {
         case ArmorCategory.Head:
            armor = Hunter.Head;
            break;
         case ArmorCategory.Arm:
            armor = Hunter.Arm;
            break;
         case ArmorCategory.Chest:
            armor = Hunter.Chest;
            break;
         case ArmorCategory.Waist:
            armor = Hunter.Waist;
            break;
         case ArmorCategory.Leg:
            armor = Hunter.Leg;
            break;
      }
      return armor;
   }

   public static void Equip(Equipment equipment)
   {
      if (equipment is Weapon weapon)
      {
         Hunter.Weapon = weapon;
      }
      else if (equipment is Armor armor)
      {
         switch (armor.Category)
         {
            case ArmorCategory.Head:
               Hunter.Head = armor;
               break;
            case ArmorCategory.Arm:
               Hunter.Arm = armor;
               break;
            case ArmorCategory.Chest:
               Hunter.Chest = armor;
               break;
            case ArmorCategory.Waist:
               Hunter.Waist = armor;
               break;
            case ArmorCategory.Leg:
               Hunter.Leg = armor;
               break;
         }
      }
   }

   public static void Unequip(Equipment equipment)
   {
      if (equipment is Weapon)
      {
         Hunter.Weapon = null;
      }
      else if (equipment is Armor armor)
      {
         switch (armor.Category)
         {
            case ArmorCategory.Head:
               Hunter.Head = null;
               break;
            case ArmorCategory.Arm:
               Hunter.Arm = null;
               break;
            case ArmorCategory.Chest:
               Hunter.Chest = null;
               break;
            case ArmorCategory.Waist:
               Hunter.Waist = null;
               break;
            case ArmorCategory.Leg:
               Hunter.Leg = null;
               break;
         }
      }
   }

   // * START - Data Methods
   /// <see cref="GameManager.SaveGame"/>
   public static GC.Dictionary<string, Variant> GetData()
   {
      GC.Dictionary<string, Variant> hunterData = new GC.Dictionary<string, Variant>()
      {
         { "Rank", Hunter.Rank },
         { "Points", Hunter.Points },
         { "PointsRequired", Hunter.PointsRequired },
         { "Zenny", Hunter.Zenny },
         { "Weapon", EquipmentManager.GetWeaponData(Hunter.Weapon) },
         { "Head", EquipmentManager.GetArmorPieceData(Hunter.Head) },
         { "Chest", EquipmentManager.GetArmorPieceData(Hunter.Chest) },
         { "Arm", EquipmentManager.GetArmorPieceData(Hunter.Arm) },
         { "Waist", EquipmentManager.GetArmorPieceData(Hunter.Waist) },
         { "Leg", EquipmentManager.GetArmorPieceData(Hunter.Leg) },
         { "MonstersSlayed", MonstersSlayed }
      };
      return hunterData;
   }

   /// <see cref="GameManager.LoadGame"/>
   public static void SetData(GC.Dictionary<string, Variant> hunterData)
   {
      Hunter.Rank = hunterData["Rank"].As<int>();
      Hunter.Points = hunterData["Points"].As<int>();
      Hunter.PointsRequired = hunterData["PointsRequired"].As<int>();
      Hunter.Zenny = hunterData["Zenny"].As<int>();

      GC.Dictionary<string, Variant> weaponData = hunterData["Weapon"].As<GC.Dictionary<string, Variant>>();
      Hunter.Weapon = EquipmentManager.GetWeaponFromData(weaponData);

      GC.Dictionary<string, Variant> headData = hunterData["Head"].As<GC.Dictionary<string, Variant>>();
      Hunter.Head = EquipmentManager.GetArmorPieceFromData(headData);

      GC.Dictionary<string, Variant> chestData = hunterData["Chest"].As<GC.Dictionary<string, Variant>>();
      Hunter.Chest = EquipmentManager.GetArmorPieceFromData(chestData);

      GC.Dictionary<string, Variant> armData = hunterData["Arm"].As<GC.Dictionary<string, Variant>>();
      Hunter.Arm = EquipmentManager.GetArmorPieceFromData(armData);

      GC.Dictionary<string, Variant> waistData = hunterData["Waist"].As<GC.Dictionary<string, Variant>>();
      Hunter.Waist = EquipmentManager.GetArmorPieceFromData(waistData);

      GC.Dictionary<string, Variant> legData = hunterData["Leg"].As<GC.Dictionary<string, Variant>>();
      Hunter.Leg = EquipmentManager.GetArmorPieceFromData(legData);

      MonstersSlayed = hunterData["MonstersSlayed"].As<GC.Dictionary<string, int>>();
   }

   /// <see cref="GameManager.DeleteGame"/>
   public static void DeleteData()
   {
      Hunter.ResetData();

      MonstersSlayed.Clear();
      FillMonstersSlayedDictionary();
   }
   // * END - Data Methods
}