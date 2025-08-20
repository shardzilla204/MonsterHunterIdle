using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class PalicoEquipmentManager : Node
{
    public List<PalicoWeapon> Weapons = new List<PalicoWeapon>();
    public List<PalicoArmor> Armor = new List<PalicoArmor>();
    public GC.Dictionary<string, Variant> Recipes = new GC.Dictionary<string, Variant>();

    public List<PalicoWeapon> CraftedWeapons = new List<PalicoWeapon>();
    public List<PalicoArmor> CraftedArmor = new List<PalicoArmor>();
    
    private string _weaponFileName = "PalicoWeapons";
    private string _armorFileName = "PalicoArmor";
    private string _equipmentFolderName = "PalicoEquipment";

    public override void _EnterTree()
    {
        MonsterHunterIdle.PalicoManager.Equipment = this;

        LoadEquipment();
        LoadRecipes();
    }

    // Go through provided file paths and load weapons and armor
    /// Uses similar methods from <see cref="EquipmentManager"/>, but iteration isn't required
    private void LoadEquipment()
    {
        AddWeapons();
        AddArmor();
    }

    private bool AddWeapons()
    {
        GC.Dictionary<string, Variant> weaponTreeDictionaries = GetEquipmentDictionaries(_weaponFileName);
        if (weaponTreeDictionaries == null) return false;

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
                PalicoWeapon palicoWeapon = new PalicoWeapon(tree);
                palicoWeapon.SetEquipment(weaponGradeDictionary);

                Weapons.Add(palicoWeapon);
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

    public PalicoWeapon GetWeapon(WeaponTree tree, int grade, int subGrade)
    {
        if (tree == WeaponTree.None) return new PalicoWeapon();

        GC.Dictionary<string, Variant> weaponTreeDictionaries = GetEquipmentDictionaries(_weaponFileName);
        if (weaponTreeDictionaries == null) return null;

        string treeString = MonsterHunterIdle.AddSpacing(tree.ToString());
        GC.Array<GC.Dictionary<string, Variant>> weaponGradeDictionaries = weaponTreeDictionaries[treeString].As<GC.Array<GC.Dictionary<string, Variant>>>();
        GC.Dictionary<string, Variant> weaponGradeDictionary = weaponGradeDictionaries[grade];

        PalicoWeapon palicoWeapon = new PalicoWeapon(tree);
        palicoWeapon.Grade = grade;
        palicoWeapon.SubGrade = subGrade;
        palicoWeapon.SetEquipment(weaponGradeDictionary);

        return palicoWeapon;
    }

    private bool AddArmor()
    {
        GC.Dictionary<string, Variant> armorSetDictionaries = GetEquipmentDictionaries(_armorFileName);
        if (armorSetDictionaries == null) return false;

        foreach (string setName in armorSetDictionaries.Keys)
        {
            string categoryNameString = armorSetDictionaries["Category"].As<string>();
            PalicoEquipmentCategory category = Enum.Parse<PalicoEquipmentCategory>(categoryNameString);

            string setNameString = setName.Replace(" ", "");
            setNameString = setNameString.Replace("-", "");
            ArmorSet set = Enum.Parse<ArmorSet>(setNameString);

            try
            {
                GC.Array<GC.Dictionary<string, Variant>> armorGradeDictionaries = armorSetDictionaries[setName].As<GC.Array<GC.Dictionary<string, Variant>>>();

                // Only get the first dictionary for crafting
                GC.Dictionary<string, Variant> armorGradeDictionary = armorGradeDictionaries[0];
                PalicoArmor palicoArmor = new PalicoArmor(category, set);
                palicoArmor.SetEquipment(armorGradeDictionary);

                Armor.Add(palicoArmor);
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

    public PalicoArmor GetArmor(PalicoEquipmentCategory category, ArmorSet set, int grade, int subGrade)
    {
        if (set == ArmorSet.None) return new PalicoArmor(category);

        GC.Dictionary<string, Variant> armorSetDictionaries = GetEquipmentDictionaries(_armorFileName);
        if (armorSetDictionaries == null) return null;

        string setString = MonsterHunterIdle.AddSpacing(set.ToString());
        GC.Array<GC.Dictionary<string, Variant>> armorGradeDictionaries = armorSetDictionaries[setString].As<GC.Array<GC.Dictionary<string, Variant>>>();
        GC.Dictionary<string, Variant> armorGradeDictionary = armorGradeDictionaries[grade];

        PalicoArmor palicoArmor = new PalicoArmor(category, set);
        palicoArmor.Grade = grade;
        palicoArmor.SubGrade = subGrade;
        palicoArmor.SetEquipment(armorGradeDictionary);

        return palicoArmor;
    }

    private GC.Dictionary<string, Variant> GetEquipmentDictionaries(string fileName)
    {
        GC.Dictionary<string, Variant> equipmentData = MonsterHunterIdle.LoadFile(fileName, _equipmentFolderName).As<GC.Dictionary<string, Variant>>();
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
        string weaponCraftingFileName = $"{_weaponFileName}Crafting";
        string armorCraftingFileName = $"{_armorFileName}Crafting";

        string craftingFolderPath = $"{_equipmentFolderName}/Crafting";
        AddRecipes(weaponCraftingFileName, craftingFolderPath);
        AddRecipes(armorCraftingFileName, craftingFolderPath);
    }

    private bool AddRecipes(string fileName, string folderPath)
    {
        try
        {
            // Group = Tree/Set
            GC.Dictionary<string, Variant> groupRecipeDictionaries = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
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
        if (equipment is PalicoWeapon targetWeapon)
        {
            PalicoWeapon weapon = CraftedWeapons.Find(weapon => weapon.Category == targetWeapon.Category && weapon.Tree == targetWeapon.Tree);
            hasCrafted = weapon != null;
        }
        else if (equipment is PalicoArmor targetArmor)
        {
            PalicoArmor armor = CraftedArmor.Find(armor => armor.Category == targetArmor.Category && armor.Set == targetArmor.Set);
            hasCrafted = armor != null;
        }
        return hasCrafted;
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
        if (equipment is PalicoWeapon weapon)
        {
            PalicoWeapon upgradedWeapon = GetWeapon(weapon.Tree, weapon.Grade, weapon.SubGrade);
            upgradedWeapon.Attack = weapon.Attack;
            upgradedWeapon.Affinity = weapon.Affinity;
            upgradedWeapon.Name = weapon.Name;
        }
        else if (equipment is PalicoArmor armor)
        {
            PalicoArmor upgradedArmor = GetArmor(armor.Category, armor.Set, armor.Grade, armor.SubGrade);
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