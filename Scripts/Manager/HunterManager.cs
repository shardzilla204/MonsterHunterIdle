using System.Collections.Generic;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class HunterManager : Node
{
   [Export]
   private int _startingZenny = 100;

   [Export]
   private GC.Array<GC.Dictionary<string, Variant>> _startingEquipment = new GC.Array<GC.Dictionary<string, Variant>>()
   {
      new GC.Dictionary<string, Variant>()
      {
         { "Type", (int) EquipmentType.Weapon },
         { "Category", (int) WeaponCategory.SwordAndShield },
         { "Tree", (int) WeaponTree.Ore },
         { "Grade", 0 },
         { "SubGrade", 0 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Type", (int) EquipmentType.Armor },
         { "Category", (int) ArmorCategory.Head },
         { "Set", (int) ArmorSet.Leather },
         { "Grade", 0 },
         { "SubGrade", 0 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Type", (int) EquipmentType.Armor },
         { "Category", (int) ArmorCategory.Chest },
         { "Set", (int) ArmorSet.Leather },
         { "Grade", 0 },
         { "SubGrade", 0 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Type", (int) EquipmentType.Armor },
         { "Category", (int) ArmorCategory.Arm },
         { "Set", (int) ArmorSet.Leather },
         { "Grade", 0 },
         { "SubGrade", 0 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Type", (int) EquipmentType.Armor },
         { "Category", (int) ArmorCategory.Waist },
         { "Set", (int) ArmorSet.Leather },
         { "Grade", 0 },
         { "SubGrade", 0 }
      },
      new GC.Dictionary<string, Variant>()
      {
         { "Type", (int) EquipmentType.Armor },
         { "Category", (int) ArmorCategory.Leg },
         { "Set", (int) ArmorSet.Leather },
         { "Grade", 0 },
         { "SubGrade", 0 }
      }
   };

   public Hunter Hunter;

   public override void _EnterTree()
   {
      MonsterHunterIdle.HunterManager = this;

      Hunter = new Hunter(_startingZenny);
   }

   private void AddStartingEquipment()
   {
      foreach (GC.Dictionary<string, Variant> equipmentDictionary in _startingEquipment)
      {
         EquipmentType equipmentType = (EquipmentType) equipmentDictionary["Type"].As<int>();
         int grade = equipmentDictionary["Grade"].As<int>();
         int subGrade = equipmentDictionary["SubGrade"].As<int>();

         if (equipmentType == EquipmentType.Weapon)
         {
            WeaponCategory category = (WeaponCategory) equipmentDictionary["Category"].As<int>();
            WeaponTree tree = (WeaponTree)equipmentDictionary["Tree"].As<int>();
            AddStartingWeapon(category, tree, grade, subGrade);
         }
         else
         {
            ArmorCategory category = (ArmorCategory) equipmentDictionary["Category"].As<int>();
            ArmorSet set = (ArmorSet)equipmentDictionary["Set"].As<int>();
            AddStartingArmor(category, set, grade, subGrade);
         }
      }
   }

   private void AddStartingWeapon(WeaponCategory category, WeaponTree tree, int grade, int subGrade)
   {
      Weapon weapon = MonsterHunterIdle.EquipmentManager.GetWeapon(category, tree, grade, subGrade);
      if (weapon == null) return;

      Hunter.Weapon = weapon;

      // Console message
      string addedWeaponMessage = $"Added Weapon {weapon.Name} | {weapon.Grade + 1}.{weapon.SubGrade + 1}";
      PrintRich.PrintLine(TextColor.Yellow, addedWeaponMessage);

      Hunter.Weapons.Add(weapon);
   }

   private void AddStartingArmor(ArmorCategory category, ArmorSet set, int grade, int subGrade)
   {
      Armor armor = MonsterHunterIdle.EquipmentManager.GetArmor(category, set, grade, subGrade);
      if (armor == null) return;

      switch (armor.Category)
      {
         case ArmorCategory.Head:
            Hunter.Head = armor;
            break;
         case ArmorCategory.Chest:
            Hunter.Chest = armor;
            break;
         case ArmorCategory.Arm:
            Hunter.Arm = armor;
            break;
         case ArmorCategory.Waist:
            Hunter.Waist = armor;
            break;
         case ArmorCategory.Leg:
            Hunter.Leg = armor;
            break;
      }

      // Console message
      string addedArmorMessage = $"Added Armor {armor.Name} | {armor.Grade + 1}.{armor.SubGrade + 1}";
      PrintRich.PrintLine(TextColor.Yellow, addedArmorMessage);

      Hunter.Armor.Add(armor);
   }

   public void AddHunterPoints(int progressAmount)
   {
      Hunter.Points += progressAmount;

      MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.HunterPointsChanged);

      CheckHunterProgress();
   }

   public void AddZenny(int zennyAmount)
   {
      Hunter.Zenny += zennyAmount;
   }

   private void CheckHunterProgress()
   {
      if (Hunter.Points < Hunter.PointsRequired || Hunter.Rank >= Hunter.MaxRank) return;

      Hunter.Rank++; // Increase Rank
      Hunter.Points -= Hunter.PointsRequired; // Reset Points
      IncreaseHunterProgress();

      MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.HunterLeveledUp);
   }

   private void IncreaseHunterProgress()
   {
      int pointsThreshold = 100;

      if (Hunter.Rank < pointsThreshold) Hunter.PointsRequired += pointsThreshold;
   }

   private Armor FindArmor(ArmorCategory armorCategory)
   {
      return Hunter.Armor.Find(armor => armor.Category == armorCategory);
   }

   public int GetHunterAttack()
   {
      int attack = Hunter.Weapon.Attack;

      RandomNumberGenerator RNG = new RandomNumberGenerator();
      float minPercent = 0.25f;
      float maxPercent = 1;
      float randomPercentage = RNG.RandfRange(minPercent, maxPercent);
      return Mathf.RoundToInt(attack * randomPercentage);
   }

   public Equipment FindWeapon(Weapon targetWeapon)
   {
      Equipment desiredWeapon = Hunter.Weapons.Find(weapon => weapon.Category == targetWeapon.Category && weapon.Tree == targetWeapon.Tree);

      if (desiredWeapon == null)
      {
         string errorMessage = $"Couldn't Find Desired Weapon Of {targetWeapon.Category} {targetWeapon.Tree}. Returning Null";
         GD.PrintErr(errorMessage);
      }

      return desiredWeapon;
   }

   public Armor FindArmor(Armor targetArmor)
   {
      Armor desiredArmor = Hunter.Armor.Find(armor => armor.Category == targetArmor.Category && armor.Set == targetArmor.Set);

      if (desiredArmor == null)
      {
         string errorMessage = $"Couldn't Find Desired Equipment Of {targetArmor.Category} {targetArmor.Set}. Returning Null";
         GD.PrintErr(errorMessage);
      }

      return desiredArmor;
   }

   public bool IsEquipped(Equipment equipment)
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

   private Armor GetArmor(ArmorCategory targetCategory)
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

   // Data methods
   /// <see cref="GameManager.SaveGame"/>
   public GC.Dictionary<string, Variant> GetData()
   {
      GC.Dictionary<string, Variant> hunterData = new GC.Dictionary<string, Variant>()
      {
         { "Rank", Hunter.Rank },
         { "Points", Hunter.Points },
         { "PointsRequired", Hunter.PointsRequired },
         { "Zenny", Hunter.Zenny },
         { "Weapons", GetWeaponsData() },
         { "Weapon", GetWeaponData(Hunter.Weapon) },
         { "Armor", GetArmorData() },
         { "Head", GetArmorPieceData(Hunter.Head) },
         { "Chest", GetArmorPieceData(Hunter.Chest) },
         { "Arm", GetArmorPieceData(Hunter.Arm) },
         { "Waist", GetArmorPieceData(Hunter.Waist) },
         { "Leg", GetArmorPieceData(Hunter.Leg) }
      };
      return hunterData;
   }

   private GC.Array<GC.Dictionary<string, Variant>> GetWeaponsData()
   {
      GC.Array<GC.Dictionary<string, Variant>> weaponsData = new GC.Array<GC.Dictionary<string, Variant>>();
      foreach (Weapon weapon in Hunter.Weapons)
      {
         GC.Dictionary<string, Variant> weaponData = GetWeaponData(weapon);
         weaponsData.Add(weaponData);
      }
      return weaponsData;
   }

   private GC.Dictionary<string, Variant> GetWeaponData(Weapon weapon)
   {
      GC.Dictionary<string, Variant> weaponData = new GC.Dictionary<string, Variant>()
      {
         { "Category", (int) weapon.Category },
         { "Tree", (int) weapon.Tree },
         { "Grade", weapon.Grade },
         { "SubGrade", weapon.SubGrade }
      };
      return weaponData;
   }

   private GC.Array<GC.Dictionary<string, Variant>> GetArmorData()
   {
      GC.Array<GC.Dictionary<string, Variant>> armorData = new GC.Array<GC.Dictionary<string, Variant>>();
      foreach (Armor armor in Hunter.Armor)
      {
         GC.Dictionary<string, Variant> armorPieceData = GetArmorPieceData(armor);
         armorData.Add(armorPieceData);
      }
      return armorData;
   }

   private GC.Dictionary<string, Variant> GetArmorPieceData(Armor armor)
   {
      GC.Dictionary<string, Variant> armorPieceData = new GC.Dictionary<string, Variant>()
      {
         { "Category", (int) armor.Category },
         { "Set", (int) armor.Set },
         { "Grade", armor.Grade },
         { "SubGrade", armor.SubGrade }
      };
      return armorPieceData;
   }

   /// <see cref="GameManager.LoadGame"/>
   public void SetData(GC.Dictionary<string, Variant> hunterData)
   {
      Hunter.Rank = hunterData["Rank"].As<int>();
      Hunter.Points = hunterData["Points"].As<int>();
      Hunter.PointsRequired = hunterData["PointsRequired"].As<int>();
      Hunter.Zenny = hunterData["Zenny"].As<int>();

      Hunter.Weapons = GetWeaponsFromData(hunterData);

      GC.Dictionary<string, Variant> weaponData = hunterData["Weapon"].As<GC.Dictionary<string, Variant>>();
      Hunter.Weapon = GetWeaponFromData(weaponData);

      Hunter.Armor = GetArmorFromData(hunterData);

      GC.Dictionary<string, Variant> headData = hunterData["Head"].As<GC.Dictionary<string, Variant>>();
      Hunter.Head = GetArmorPieceFromData(headData);

      GC.Dictionary<string, Variant> chestData = hunterData["Chest"].As<GC.Dictionary<string, Variant>>();
      Hunter.Chest = GetArmorPieceFromData(chestData);

      GC.Dictionary<string, Variant> armData = hunterData["Arm"].As<GC.Dictionary<string, Variant>>();
      Hunter.Arm = GetArmorPieceFromData(armData);

      GC.Dictionary<string, Variant> waistData = hunterData["Waist"].As<GC.Dictionary<string, Variant>>();
      Hunter.Waist = GetArmorPieceFromData(waistData);

      GC.Dictionary<string, Variant> legData = hunterData["Leg"].As<GC.Dictionary<string, Variant>>();
      Hunter.Leg = GetArmorPieceFromData(legData);
   }

   private List<Weapon> GetWeaponsFromData(GC.Dictionary<string, Variant> hunterData)
   {
      List<Weapon> weapons = new List<Weapon>();
      GC.Array<GC.Dictionary<string, Variant>> weaponsData = hunterData["Weapons"].As<GC.Array<GC.Dictionary<string, Variant>>>();
      foreach (GC.Dictionary<string, Variant> weaponData in weaponsData)
      {
         Weapon weapon = GetWeaponFromData(weaponData);
         weapons.Add(weapon);
      }
      return weapons;
   }

   private Weapon GetWeaponFromData(GC.Dictionary<string, Variant> weaponData)
   {
      WeaponCategory category = (WeaponCategory)weaponData["Category"].As<int>();
      WeaponTree tree = (WeaponTree)weaponData["Tree"].As<int>();
      int grade = weaponData["Grade"].As<int>();
      int subGrade = weaponData["SubGrade"].As<int>();

      Weapon weapon = MonsterHunterIdle.EquipmentManager.GetWeapon(category, tree, grade, subGrade);
      return weapon;
   }

   private List<Armor> GetArmorFromData(GC.Dictionary<string, Variant> hunterData)
   {
      List<Armor> armor = new List<Armor>();
      GC.Array<GC.Dictionary<string, Variant>> armorData = hunterData["Armor"].As<GC.Array<GC.Dictionary<string, Variant>>>();
      foreach (GC.Dictionary<string, Variant> armorPieceData in armorData)
      {
         Armor armorPiece = GetArmorPieceFromData(armorPieceData);
         armor.Add(armorPiece);
      }
      return armor;
   }

   private Armor GetArmorPieceFromData(GC.Dictionary<string, Variant> armorData)
   {
      ArmorCategory category = (ArmorCategory) armorData["Category"].As<int>();
      ArmorSet set = (ArmorSet) armorData["Set"].As<int>();
      int grade = armorData["Grade"].As<int>();
      int subGrade = armorData["SubGrade"].As<int>();

      Armor armorPiece = MonsterHunterIdle.EquipmentManager.GetArmor(category, set, grade, subGrade);
      return armorPiece;
   }

   /// <see cref="GameManager.DeleteGame"/>
   public void DeleteData()
   {
      Hunter = new Hunter(_startingZenny);

      AddStartingEquipment();
   }
}