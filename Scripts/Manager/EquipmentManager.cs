using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public enum EquipmentType
{
    None = -1,
    Weapon,
    Armor
}

public enum EntityType
{
    Player,
    Palico
}

public partial class EquipmentManager : Node
{
    public List<Armor> Armor = new List<Armor>();
    public List<Weapon> Weapons = new List<Weapon>();
    public GC.Dictionary<string, Variant> Recipes = new GC.Dictionary<string, Variant>();

    public override void _EnterTree()
    {
        MonsterHunterIdle.EquipmentManager = this;

        LoadEquipment();
        LoadRecipes();
    }

    // Go through provided file paths and load weapons and armor
    private void LoadEquipment()
    {
        string weaponFilePath = "res://JSON/Weapons/";
        LoadEquipment(weaponFilePath, AddWeapons);

        string armorFilePath = "res://JSON/Armor/";
        LoadEquipment(armorFilePath, AddArmor);
    }

    private void LoadEquipment(string filePath, Func<string, bool> addEquipment)
    {
        using DirAccess directory = DirAccess.Open(filePath);
        if (directory != null)
        {
            directory.ListDirBegin();
            string fileName = directory.GetNext();
            while (fileName != "")
            {
                bool hasPassed = addEquipment(fileName.GetBaseName());
                if (!hasPassed) return;

                fileName = directory.GetNext();
            }
        }
        else
        {
            string errorMessage = $"Couldn't Access Using The Path: {filePath}";
            GD.PrintErr(errorMessage);
        }
    }

    private bool AddWeapons(string fileName)
    {
        GC.Dictionary<string, Variant> weaponTreeDictionaries = GetEquipmentDictionaries(fileName, "Weapons");
        if (weaponTreeDictionaries == null) return false;

        WeaponCategory category = Enum.Parse<WeaponCategory>(fileName);
        foreach (string treeName in weaponTreeDictionaries.Keys)
        {
            string treeNameString = treeName.Replace(" ", "");
            treeNameString = treeNameString.Replace("-", "");
            
            WeaponTree tree = Enum.Parse<WeaponTree>(treeNameString);
            
            try
            {
                GC.Array<GC.Dictionary<string, Variant>> weaponGradeDictionaries = weaponTreeDictionaries[treeName].As<GC.Array<GC.Dictionary<string, Variant>>>();

                // Only get the first dictionary for crafting
                GC.Dictionary<string, Variant> weaponGradeDictionary = weaponGradeDictionaries[0];
                Weapon weapon = new Weapon(category, tree);
                weapon.SetEquipment(weaponGradeDictionary);

                Weapons.Add(weapon);
            }
            catch (Exception exception)
            {
                string errorMessage = exception.ToString();
                if (exception.Message.Trim() != "") errorMessage = exception.Message;

                GD.PrintErr(errorMessage);

                return false;
            }
        }
        return true;
    }

    public Weapon GetWeapon(WeaponCategory category, WeaponTree tree, int grade, int subGrade)
    {
        if (tree == WeaponTree.None) return new Weapon();

        string fileName = category.ToString();
        GC.Dictionary<string, Variant> weaponTreeDictionaries = GetEquipmentDictionaries(fileName, "Weapons");
        if (weaponTreeDictionaries == null) return null;

        string treeString = MonsterHunterIdle.AddSpacing(tree.ToString());
        GC.Array<GC.Dictionary<string, Variant>> weaponGradeDictionaries = weaponTreeDictionaries[treeString].As<GC.Array<GC.Dictionary<string, Variant>>>();
        GC.Dictionary<string, Variant> weaponGradeDictionary = weaponGradeDictionaries[grade];

        Weapon weapon = new Weapon(category, tree);
        weapon.Grade = grade;
        weapon.SubGrade = subGrade;
        weapon.SetEquipment(weaponGradeDictionary);

        return weapon;
    }

    private bool AddArmor(string fileName)
    {
        GC.Dictionary<string, Variant> armorSetDictionaries = GetEquipmentDictionaries(fileName, "Armor");
        if (armorSetDictionaries == null) return false;

        string categoryName = fileName.Replace("Armor", "");
        ArmorCategory category = Enum.Parse<ArmorCategory>(categoryName);

        foreach (string setName in armorSetDictionaries.Keys)
        {
            string setNameString = setName.Replace(" ", "");
            setNameString = setNameString.Replace("-", "");
            ArmorSet set = Enum.Parse<ArmorSet>(setNameString);

            try
            {
                GC.Array<GC.Dictionary<string, Variant>> armorGradeDictionaries = armorSetDictionaries[setName].As<GC.Array<GC.Dictionary<string, Variant>>>();

                // Only get the first dictionary for crafting
                GC.Dictionary<string, Variant> armorGradeDictionary = armorGradeDictionaries[0];
                Armor armor = new Armor(category, set);
                armor.SetEquipment(armorGradeDictionary);

                Armor.Add(armor);
            }
            catch (Exception exception)
            {
                string errorMessage = exception.ToString();
                if (exception.Message.Trim() != "") errorMessage = exception.Message;

                GD.PrintErr(errorMessage);

                return false;
            }
        }
        return true;
    }

    public Armor GetArmor(ArmorCategory category, ArmorSet set, int grade, int subGrade)
    {
        if (set == ArmorSet.None) return new Armor(category, set);

        string fileName = $"{category}Armor";
        GC.Dictionary<string, Variant> armorSetDictionaries = GetEquipmentDictionaries(fileName, "Armor");
        if (armorSetDictionaries == null) return null;

        string setString = MonsterHunterIdle.AddSpacing(set.ToString());
        GC.Array<GC.Dictionary<string, Variant>> armorGradeDictionaries = armorSetDictionaries[setString].As<GC.Array<GC.Dictionary<string, Variant>>>();
        GC.Dictionary<string, Variant> armorGradeDictionary = armorGradeDictionaries[grade];

        Armor armor = new Armor(category, set);
        armor.Grade = grade;
        armor.SubGrade = subGrade;
        armor.SetEquipment(armorGradeDictionary);

        return armor;
    }

    private GC.Dictionary<string, Variant> GetEquipmentDictionaries(string fileName, string folderName)
    {
        GC.Dictionary<string, Variant> equipmentData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        if (equipmentData == null) return null;

        try
        {
            GC.Dictionary<string, Variant> equipmentDictionaries = equipmentData[fileName].As<GC.Dictionary<string, Variant>>();
            return equipmentDictionaries;
        }
        catch (Exception exception)
        {
            string errorMessage = exception.ToString();
            if (exception.Message.Trim() != "") errorMessage = exception.Message;

            GD.PrintErr(errorMessage);

            return null;
        }
    }

    // Go through provided file paths and load weapons and armor
    private void LoadRecipes()
    {
        string weaponFolderName = "WeaponCrafting";
        string weaponFilePath = $"res://JSON/{weaponFolderName}/";
        LoadRecipe(weaponFilePath, weaponFolderName, AddRecipes);

        string armorFolderName = "ArmorCrafting";
        string armorFilePath = $"res://JSON/{armorFolderName}/";
        LoadRecipe(armorFilePath, armorFolderName, AddRecipes);
    }

    private void LoadRecipe(string filePath, string folderName, Func<string, string, bool> addRecipe)
    {
        using DirAccess directory = DirAccess.Open(filePath);
        if (directory != null)
        {
            directory.ListDirBegin();
            string fileName = directory.GetNext();
            while (fileName != "")
            {
                bool hasPassed = addRecipe(fileName.GetBaseName(), folderName);
                if (!hasPassed) return;

                fileName = directory.GetNext();
            }
        }
        else
        {
            string errorMessage = $"Couldn't Access Using The Path: {filePath}";
            GD.PrintErr(errorMessage);
        }
    }

    private bool AddRecipes(string fileName, string folderName)
    {
        try
        {
            // Group = Tree/Set
            GC.Dictionary<string, Variant> groupRecipeDictionaries = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
            if (groupRecipeDictionaries == null) return false;

            foreach (string groupName in groupRecipeDictionaries.Keys)
            {
                Recipes.Add(groupName, groupRecipeDictionaries[groupName]);
            }
        }
        catch (Exception exception)
        {
            string errorMessage = exception.ToString();
            if (exception.Message.Trim() != "") errorMessage = exception.Message;

            GD.PrintErr(errorMessage);

            return false;
        }
        return true;
    }

    public bool HasCrafted(Equipment equipment)
    {
        bool hasCrafted = false;
        if (equipment is Weapon targetWeapon)
        {
            Weapon weapon = MonsterHunterIdle.HunterManager.Hunter.Weapons.Find(weapon => weapon.Category == targetWeapon.Category && weapon.Tree == targetWeapon.Tree);
            hasCrafted = weapon != null;
        }
        else if (equipment is Armor targetArmor)
        {
            Armor armor = MonsterHunterIdle.HunterManager.Hunter.Armor.Find(armor => armor.Category == targetArmor.Category && armor.Set == targetArmor.Set);
            hasCrafted = armor != null;
        }
        return hasCrafted;
    }

    public Equipment GetHunterEquipment(Equipment equipment)
    {
        if (equipment is Weapon targetWeapon)
        {
            Weapon weapon = MonsterHunterIdle.HunterManager.Hunter.Weapons.Find(weapon => weapon.Category == targetWeapon.Category && weapon.Tree == targetWeapon.Tree);
            PrintRich.PrintEquipmentInfo(TextColor.Orange, weapon);
            return weapon;
        }
        else if (equipment is Armor targetArmor)
        {
            Armor armor = MonsterHunterIdle.HunterManager.Hunter.Armor.Find(armor => armor.Category == targetArmor.Category && armor.Set == targetArmor.Set);
            PrintRich.PrintEquipmentInfo(TextColor.Orange, armor);
            return armor;
        }

        string errorMessage = "Couldn't Find Equipment Type. Returning Null";
        GD.PrintErr(errorMessage);

        return null;
    }

    public List<GC.Dictionary<string, Variant>> FindRecipe(Equipment equipment, bool getNextRecipe = false)
    {
        int maxSubGrade = 4;
        string categoryName = "";
        string groupName = "";
        int grade = equipment.Grade;
        int subGrade = getNextRecipe ? equipment.SubGrade + 1 : equipment.SubGrade;

        if (subGrade > maxSubGrade)
        {
            grade++;
            subGrade = 0;
        }

        // Get the group & category of the equipment
        List<string> categoryNames = Recipes.Keys.ToList();
        if (equipment is Weapon weapon)
            {
                string weaponTreeString = weapon.Tree.ToString();
                groupName = MonsterHunterIdle.AddSpacing(weaponTreeString);

                categoryName = categoryNames.Find(name => name.Contains(weapon.Category.ToString()));
            }
            else if (equipment is Armor armor)
            {
                string armorSetString = armor.Set.ToString();
                groupName = MonsterHunterIdle.AddSpacing(armorSetString);

                categoryName = categoryNames.Find(name => name.Contains(armor.Category.ToString()));
            }

        // Create a dictionary for the recipe 
        try
        {
            // -> e.g SwordAndShieldCrafting
            GC.Dictionary<string, Variant> categoryRecipeDictionary = Recipes[categoryName].As<GC.Dictionary<string, Variant>>();

            // -> e.g Ore
            List<GC.Dictionary<string, Variant>> groupRecipeDictionaries = categoryRecipeDictionary[groupName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();
            GC.Dictionary<string, Variant> groupRecipeDictionary = groupRecipeDictionaries[grade];

            // -> e.g Materials
            List<GC.Array<GC.Dictionary<string, Variant>>> materialDictionaries = groupRecipeDictionary["Materials"].As<GC.Array<GC.Array<GC.Dictionary<string, Variant>>>>().ToList();
            List<GC.Dictionary<string, Variant>> recipe = materialDictionaries[subGrade].ToList();

            return recipe;
        }
        catch (Exception exception)
        {
            string errorMessage = "Couldn't Find Recipe. Returning Null";
            if (exception.Message.Trim() != "") errorMessage = exception.Message;

            GD.PrintErr(errorMessage);

            return null;
        }
    }

    private void GetWeaponAttack(Weapon weapon)
    {

    }

    public void UpgradeEquipment(Equipment equipment)
    {
        equipment.SubGrade++;

        int maxSubGrade = 4;
        if (equipment.SubGrade > maxSubGrade)
        {
            equipment.Grade++;
            equipment.SubGrade = 0;
        }

        // Set the details of the equipment
        if (equipment is Weapon weapon)
        {
            Weapon upgradedWeapon = GetWeapon(weapon.Category, weapon.Tree, weapon.Grade, weapon.SubGrade);
            weapon.Attack = upgradedWeapon.Attack;
            weapon.Affinity = upgradedWeapon.Affinity;
            weapon.Name = upgradedWeapon.Name;
        }
        else if (equipment is Armor armor)
        {
            Armor upgradedArmor = GetArmor(armor.Category, armor.Set, armor.Grade, armor.SubGrade);
            armor.Defense = upgradedArmor.Defense;
            armor.Name = upgradedArmor.Name;
        }
        else
        {
            string errorMessage = $"Couldn't Upgrade {equipment.Name}";
            GD.PrintErr(errorMessage);
        }
    }
}