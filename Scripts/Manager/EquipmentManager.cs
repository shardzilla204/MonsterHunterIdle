using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class EquipmentManager : Node
{
    public List<Weapon> Weapons = new List<Weapon>();
    public List<Armor> Armor = new List<Armor>();
    public GC.Dictionary<string, Variant> Recipes = new GC.Dictionary<string, Variant>();

    public List<Weapon> CraftedWeapons = new List<Weapon>();
    public List<Armor> CraftedArmor = new List<Armor>();

    private string _weaponFolderPath = "Equipment/Weapons";
    private string _armorFolderPath = "Equipment/Armor";

    public override void _EnterTree()
    {
        MonsterHunterIdle.EquipmentManager = this;

        LoadEquipment();
        LoadRecipes();
    }

    // Go through provided file paths and load weapons and armor
    private void LoadEquipment()
    {
        string weaponFilePath = $"res://JSON/{_weaponFolderPath}/";
        LoadEquipment(weaponFilePath, _weaponFolderPath, AddWeapons);

        string armorFilePath = $"res://JSON/{_armorFolderPath}/";
        LoadEquipment(armorFilePath, _armorFolderPath, AddArmor);
    }

    /// Take in a file path, folder path, and method defending on the equipment | Weapons = <see cref="AddWeapons"/> | Armor = <see cref="AddArmor"/>
    /// Iterate through the provided file path e.g "res://JSON/Equipment/Weapons/".
    /// Take the file name without the extension (<see cref="string.GetBaseName()"/>) 
    /// Use fileName and folderPath (<see cref="LoadEquipment.folderPath"/>) as parameters in the provided method
    /// If successful, iterate to the next file
    /// <see cref="LoadRecipe"/> uses this method as well
    private void LoadEquipment(string filePath, string folderPath, Func<string, string, bool> addEquipment)
    {
        using DirAccess directory = DirAccess.Open(filePath);
        if (directory != null)
        {
            directory.ListDirBegin();
            string fileName = directory.GetNext();
            while (fileName != "")
            {
                if (directory.CurrentIsDir())
                {
                    fileName = directory.GetNext();
                    continue;
                }

                bool hasPassed = addEquipment(fileName.GetBaseName(), folderPath);
                if (!hasPassed)
                {
                    string errorMessage = $"Couldn't Add Equipment Using The Path: {filePath}";
                    GD.PrintErr(errorMessage);
                    return;
                }

                fileName = directory.GetNext();
            }
        }
        else
        {
            string errorMessage = $"Couldn't Access Using The Path: {filePath}";
            GD.PrintErr(errorMessage);
        }
    }

    private bool AddWeapons(string fileName, string folderPath)
    {
        GC.Dictionary<string, Variant> weaponTreeDictionaries = GetEquipmentDictionaries(fileName, folderPath);
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

    public Weapon GetWeapon(WeaponCategory category, WeaponTree tree, int grade = 0, int subGrade = 0)
    {
        if (tree == WeaponTree.None) return new Weapon();

        string fileName = category.ToString();
        GC.Dictionary<string, Variant> weaponTreeDictionaries = GetEquipmentDictionaries(fileName, _weaponFolderPath);
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

    private bool AddArmor(string fileName, string folderPath)
    {
        GC.Dictionary<string, Variant> armorSetDictionaries = GetEquipmentDictionaries(fileName, folderPath);
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
        GC.Dictionary<string, Variant> armorSetDictionaries = GetEquipmentDictionaries(fileName, _armorFolderPath);
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

    private GC.Dictionary<string, Variant> GetEquipmentDictionaries(string fileName, string folderPath)
    {
        GC.Dictionary<string, Variant> equipmentData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
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

    // Go through provided file paths and load recipes for weapons and armor
    private void LoadRecipes()
    {
        string weaponCraftingFolderPath = $"{_weaponFolderPath}/Crafting";
        string weaponCraftingFilePath = $"res://JSON/{weaponCraftingFolderPath}/";
        LoadRecipe(weaponCraftingFilePath, weaponCraftingFolderPath, AddRecipes);

        string armorCraftingFolderPath = $"{_armorFolderPath}/Crafting";
        string armorCraftingFilePath = $"res://JSON/{armorCraftingFolderPath}/";
        LoadRecipe(armorCraftingFilePath, armorCraftingFolderPath, AddRecipes);
    }

    private void LoadRecipe(string filePath, string folderPath, Func<string, string, bool> addRecipe)
    {
        using DirAccess directory = DirAccess.Open(filePath);
        if (directory != null)
        {
            directory.ListDirBegin();
            string fileName = directory.GetNext();
            while (fileName != "")
            {
                if (directory.CurrentIsDir())
                {
                    fileName = directory.GetNext();
                    continue;
                }

                bool hasPassed = addRecipe(fileName.GetBaseName(), folderPath);
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
        if (equipment is Weapon targetWeapon)
        {
            Weapon weapon = CraftedWeapons.Find(weapon => weapon.Category == targetWeapon.Category && weapon.Tree == targetWeapon.Tree);
            hasCrafted = weapon != null;
        }
        else if (equipment is Armor targetArmor)
        {
            Armor armor = CraftedArmor.Find(armor => armor.Category == targetArmor.Category && armor.Set == targetArmor.Set);
            hasCrafted = armor != null;
        }
        return hasCrafted;
    }

    public Equipment GetHunterEquipment(Equipment equipment)
    {
        if (equipment is Weapon targetWeapon)
        {
            Weapon weapon = CraftedWeapons.Find(weapon => weapon.Category == targetWeapon.Category && weapon.Tree == targetWeapon.Tree);
            PrintRich.PrintEquipmentInfo(TextColor.Orange, weapon);
            return weapon;
        }
        else if (equipment is Armor targetArmor)
        {
            Armor armor = CraftedArmor.Find(armor => armor.Category == targetArmor.Category && armor.Set == targetArmor.Set);
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
        string categoryName = GetCategoryName(equipment);
        string groupName = GetGroupName(equipment);
        int grade = equipment.Grade;
        int subGrade = getNextRecipe ? equipment.SubGrade + 1 : equipment.SubGrade;

        // Move onto the next grade
        if (subGrade > maxSubGrade && grade < maxSubGrade)
        {
            grade++;
            subGrade = 0;
        }

        // Create a dictionary for the recipe 
        try
        {
            // SwordAndShieldCrafting -> Great Jagras
            GC.Dictionary<string, Variant> groupDictionaries = Recipes[categoryName].As<GC.Dictionary<string, Variant>>();

            // Great Jagras -> Grade 2
            List<Variant> gradeDictionaries = groupDictionaries[groupName].As<GC.Array<Variant>>().ToList();

            // Grade 2 -> SubGrade 1
            List<Variant> subGradeDictionaries = gradeDictionaries[grade].As<GC.Array<Variant>>().ToList();

            // SubGrade 1 -> Recipe | Great Jagras SwordAndShield (3.2)
            List<GC.Dictionary<string, Variant>> recipe = subGradeDictionaries[subGrade].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();

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

    private string GetCategoryName(Equipment equipment)
    {
        string categoryName = "";
        List<string> categoryNames = Recipes.Keys.ToList();
        if (equipment is Weapon weapon)
        {
            categoryName = categoryNames.Find(name => name.Contains(weapon.Category.ToString()));
        }
        else if (equipment is Armor armor)
        {
            categoryName = categoryNames.Find(name => name.Contains(armor.Category.ToString()));
        }
        return categoryName;
    }

    private string GetGroupName(Equipment equipment)
    {
        string groupName = "";
        if (equipment is Weapon weapon)
        {
            string weaponTreeString = weapon.Tree.ToString();
            groupName = MonsterHunterIdle.AddSpacing(weaponTreeString);
        }
        else if (equipment is Armor armor)
        {
            string armorSetString = armor.Set.ToString();
            groupName = MonsterHunterIdle.AddSpacing(armorSetString);
        }
        return groupName;
    }

    public int GetCraftingCost(int grade = 0, int subGrade = 0)
    {
        string fileName = "CraftingCost";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> craftingCostData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        if (craftingCostData == null) return 0;

        GC.Array<GC.Array<int>> gradeCosts = craftingCostData[fileName].As<GC.Array<GC.Array<int>>>();
        GC.Array<int> subGradeCosts = gradeCosts[grade];
        int craftingCost = subGradeCosts[subGrade];

        return craftingCost;
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

    public int GetDefenseValue(int grade, int subGrade = 0)
    {
        string fileName = "ArmorDefense";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> defenseData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Array<GC.Array<int>> grades = defenseData[fileName].As<GC.Array<GC.Array<int>>>();

        GC.Array<int> defenseValues = grades[grade];
        int defenseValue = defenseValues[subGrade];

        return defenseValue;
    }

    // Data methods
    /// <see cref="GameManager.SaveGame"/>
    public GC.Dictionary<string, Variant> GetData()
    {
        GC.Dictionary<string, Variant> equipmentData = new GC.Dictionary<string, Variant>()
        {
            { "Weapons", GetWeaponsData() },
            { "Armor", GetArmorData() }
        };
        return equipmentData;
    }

    private GC.Array<GC.Dictionary<string, Variant>> GetWeaponsData()
    {
        GC.Array<GC.Dictionary<string, Variant>> weaponsData = new GC.Array<GC.Dictionary<string, Variant>>();
        foreach (Weapon weapon in CraftedWeapons)
        {
            GC.Dictionary<string, Variant> weaponData = GetWeaponData(weapon);
            weaponsData.Add(weaponData);
        }
        return weaponsData;
    }

    public GC.Dictionary<string, Variant> GetWeaponData(Weapon weapon)
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
        foreach (Armor armor in CraftedArmor)
        {
            GC.Dictionary<string, Variant> armorPieceData = GetArmorPieceData(armor);
            armorData.Add(armorPieceData);
        }
        return armorData;
    }

    public GC.Dictionary<string, Variant> GetArmorPieceData(Armor armor)
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
    public void SetData(GC.Dictionary<string, Variant> equipmentData)
    {
        CraftedWeapons.AddRange(GetWeaponsFromData(equipmentData));
        CraftedArmor.AddRange(GetArmorFromData(equipmentData));
    }

    private List<Weapon> GetWeaponsFromData(GC.Dictionary<string, Variant> equipmentData)
    {
        List<Weapon> weapons = new List<Weapon>();
        GC.Array<GC.Dictionary<string, Variant>> weaponsData = equipmentData["Weapons"].As<GC.Array<GC.Dictionary<string, Variant>>>();
        foreach (GC.Dictionary<string, Variant> weaponData in weaponsData)
        {
            Weapon weapon = GetWeaponFromData(weaponData);
            weapons.Add(weapon);
        }
        return weapons;
    }

    public Weapon GetWeaponFromData(GC.Dictionary<string, Variant> weaponData)
    {
        WeaponCategory category = (WeaponCategory) weaponData["Category"].As<int>();
        WeaponTree tree = (WeaponTree) weaponData["Tree"].As<int>();
        int grade = weaponData["Grade"].As<int>();
        int subGrade = weaponData["SubGrade"].As<int>();

        Weapon weapon = GetWeapon(category, tree, grade, subGrade);
        return weapon;
    }

    // public PalicoWeapon GetPalicoWeaponFromData(GC.Dictionary<string, Variant> weaponData)
    // {
    //      WeaponCategory category = (WeaponCategory) weaponData["Category"].As<int>();
    //     WeaponTree tree = (WeaponTree) weaponData["Tree"].As<int>();
    //     int grade = weaponData["Grade"].As<int>();
    //     int subGrade = weaponData["SubGrade"].As<int>();

    //     PalicoWeapon weapon = GetWeapon(category, tree, grade, subGrade);
    //     return weapon;
    // }

    private List<Armor> GetArmorFromData(GC.Dictionary<string, Variant> equipmentData)
    {
        List<Armor> armor = new List<Armor>();
        GC.Array<GC.Dictionary<string, Variant>> armorData = equipmentData["Armor"].As<GC.Array<GC.Dictionary<string, Variant>>>();
        foreach (GC.Dictionary<string, Variant> armorPieceData in armorData)
        {
            Armor armorPiece = GetArmorPieceFromData(armorPieceData);
            armor.Add(armorPiece);
        }
        return armor;
    }

    public Armor GetArmorPieceFromData(GC.Dictionary<string, Variant> armorData)
    {
        ArmorCategory category = (ArmorCategory) armorData["Category"].As<int>();
        ArmorSet set = (ArmorSet) armorData["Set"].As<int>();
        int grade = armorData["Grade"].As<int>();
        int subGrade = armorData["SubGrade"].As<int>();

        Armor armorPiece = GetArmor(category, set, grade, subGrade);
        return armorPiece;
    }
    
    /// <see cref="GameManager.DeleteGame"/>
    public void DeleteData()
    {
        CraftedWeapons.Clear();
        CraftedArmor.Clear();
    }
}