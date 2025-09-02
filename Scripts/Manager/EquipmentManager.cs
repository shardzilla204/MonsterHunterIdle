using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class EquipmentManager : Node
{
    public static List<Weapon> Weapons = new List<Weapon>();
    public static List<Armor> Armor = new List<Armor>();

    public static GC.Dictionary<string, Variant> Recipes = new GC.Dictionary<string, Variant>();

    public static List<Weapon> CraftedWeapons = new List<Weapon>();
    public static List<Armor> CraftedArmor = new List<Armor>();

    private static string _weaponFolderPath = "Equipment/Weapons";

    private const int _MaxGrade = 9;
    private const int _MaxSubGrade = 4;

    public override void _EnterTree()
    {
        LoadEquipment();
    }

    // * START - File Methods
    private static void LoadEquipment()
    {
        string weaponFilePath = $"res://JSON/{_weaponFolderPath}/";
        LoadWeapons(weaponFilePath);
        LoadArmor();
    }

    /// Iterate through the provided file path e.g "res://JSON/Equipment/Weapons/".
    /// Take the file name without the extension (<see cref="string.GetBaseName()"/>) 
    /// Use fileName and folderPath (<see cref="LoadWeapons.folderPath"/>) as parameters in the provided method
    /// If successful, iterate to the next file
    private static void LoadWeapons(string filePath)
    {
        using DirAccess directory = DirAccess.Open(filePath);
        if (directory != null)
        {
            directory.ListDirBegin();
            string elementName = directory.GetNext();
            while (elementName != "")
            {
                if (directory.CurrentIsDir())
                {
                    elementName = directory.GetNext();
                    continue;
                }

                bool hasPassed = AddWeapons(elementName.GetBaseName());
                if (!hasPassed)
                {
                    string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                    string message = $"Couldn't Add Weapon Using The Path - {filePath}";
                    PrintRich.PrintError(className, message);

                    return;
                }

                elementName = directory.GetNext();
            }
        }
        else
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Access Using The Path: {filePath}";
            PrintRich.PrintError(className, message);
        }
    }

    private static bool AddWeapons(string fileName)
    {
        GC.Dictionary<string, Variant> weaponTreeData = MonsterHunterIdle.LoadFile(fileName, _weaponFolderPath).As<GC.Dictionary<string, Variant>>();
        if (weaponTreeData == null) return false;

        GC.Dictionary<string, Variant> weaponTreeDictionaries = weaponTreeData[fileName].As<GC.Dictionary<string, Variant>>();

        WeaponCategory category = Enum.Parse<WeaponCategory>(fileName);
        foreach (string treeName in weaponTreeDictionaries.Keys)
        {
            string treeNameString = treeName.Replace(" ", "");
            treeNameString = treeNameString.Replace("-", "");

            WeaponTree tree = Enum.Parse<WeaponTree>(treeNameString);

            try
            {
                GC.Dictionary<string, Variant> weaponTreeDictionary = weaponTreeDictionaries[treeName].As<GC.Dictionary<string, Variant>>();

                Weapon weapon = new Weapon(category, tree);
                weapon.SetEquipment(weaponTreeDictionary);

                Weapons.Add(weapon);
            }
            catch
            {
                string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
                string message = $"";
                PrintRich.PrintError(className, message);

                return false;
            }
        }
        return true;
    }

    private static void LoadArmor()
    {
        string fileName = "Armor";
        string folderPath = "Equipment/Armor";

        GC.Dictionary<string, Variant> armorData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
        if (armorData == null) return;

        GC.Dictionary<string, Variant> setDictionaries = armorData[fileName].As<GC.Dictionary<string, Variant>>();
        foreach (string setName in setDictionaries.Keys)
        {
            ArmorSet set = Enum.Parse<ArmorSet>(setName);
            GC.Array<string> setDictionary = setDictionaries[setName].As<GC.Array<string>>();
            for (int categoryIndex = 0; categoryIndex < setDictionary.Count; categoryIndex++)
            {
                ArmorCategory category = (ArmorCategory)categoryIndex;
                Armor armor = new Armor(category, set);
                armor.SetEquipment(setDictionaries);

                Armor.Add(armor);
            }
        }
    }
    // * END - File Methods

    // * START - Class Methods
    public static Weapon GetWeapon(WeaponCategory category, WeaponTree tree, int grade = 0, int subGrade = 0)
    {
        if (tree == WeaponTree.None) return new Weapon();

        string fileName = category.ToString();
        GC.Dictionary<string, Variant> weaponTreeData = MonsterHunterIdle.LoadFile(fileName, _weaponFolderPath).As<GC.Dictionary<string, Variant>>();
        if (weaponTreeData == null) return null;

        GC.Dictionary<string, Variant> weaponTreeDictionaries = weaponTreeData[fileName].As<GC.Dictionary<string, Variant>>();

        string treeString = MonsterHunterIdle.AddSpacing(tree.ToString());

        GC.Dictionary<string, Variant> weaponGradeDictionary = weaponTreeDictionaries[treeString].As<GC.Dictionary<string, Variant>>();

        Weapon weapon = new Weapon(category, tree);
        weapon.Grade = grade;
        weapon.SubGrade = subGrade;
        weapon.SetEquipment(weaponGradeDictionary);

        return weapon;
    }

    public static Armor GetArmor(ArmorCategory category, ArmorSet set, int grade, int subGrade)
    {
        if (set == ArmorSet.None) return new Armor(category, set);

        string fileName = $"Armor";
        string folderPath = "Equipment/Armor";
        GC.Dictionary<string, Variant> armorData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
        if (armorData == null) return null;

        GC.Dictionary<string, Variant> setDictionaries = armorData[fileName].As<GC.Dictionary<string, Variant>>();

        Armor armor = new Armor(category, set);
        armor.Grade = grade;
        armor.SubGrade = subGrade;
        armor.SetEquipment(setDictionaries);

        return armor;
    }

    // If the weapon's grade is past 4, then get the next name
    public static void SetWeaponName(Weapon weapon, GC.Array<string> names)
    {
        int grade = weapon.Grade;
        string romanNumeral = MonsterHunterIdle.GetRomanNumeral(grade);
        if (weapon.Grade < _MaxSubGrade)
        {
            weapon.Name = $"{names[0]} {romanNumeral}";
        }
        else
        {
            grade -= _MaxSubGrade;
            romanNumeral = MonsterHunterIdle.GetRomanNumeral(grade);
            weapon.Name = $"{names[1]} {romanNumeral}";
        }
    }

    public static void SetArmorName(Armor armor, GC.Array<string> names)
    {
        int armorCategoryIndex = (int) armor.Category;
        string romanNumeral = MonsterHunterIdle.GetRomanNumeral(armor.Grade);
        armor.Name = $"{armor.Set} {names[armorCategoryIndex]} {romanNumeral}";
    }

    public static bool HasCrafted(Equipment equipment)
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

    public static Equipment GetHunterEquipment(Equipment equipment)
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

        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        string message = $"Couldn't Find Equipment Type";
        string result = $"Returning Null";
        PrintRich.PrintError(className, message, result);

        return null;
    }

    public static int GetCraftingCost(int grade, int subGrade)
    {
        if (subGrade > _MaxSubGrade && grade < _MaxGrade)
        {
            grade++;
            subGrade = 0;
        }
        else
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = "Equipment Is Maxed Out";
            string result = "Returning 0";
            PrintRich.PrintError(className, message, result);

            return 0;
        }

        int craftingCost = MonsterHunterIdle.GetCraftingCost(grade, subGrade);
        return craftingCost;
    }

    public static void UpgradeEquipment(Equipment equipment)
    {
        equipment.SubGrade++;

        if (equipment.SubGrade > _MaxSubGrade && equipment.Grade < _MaxGrade)
        {
            equipment.Grade++;
            equipment.SubGrade = 0;
        }

        // Set the details of the equipment
        if (equipment is Weapon weapon)
        {
            Weapon upgradedWeapon = GetWeapon(weapon.Category, weapon.Tree, weapon.Grade, weapon.SubGrade);
            weapon.Name = upgradedWeapon.Name;
            weapon.Attack = upgradedWeapon.Attack;
            weapon.SpecialAttack = upgradedWeapon.SpecialAttack;
            weapon.Affinity = upgradedWeapon.Affinity;
        }
        else if (equipment is Armor armor)
        {
            Armor upgradedArmor = GetArmor(armor.Category, armor.Set, armor.Grade, armor.SubGrade);
            armor.Name = upgradedArmor.Name;
            armor.Defense = upgradedArmor.Defense;
        }
        else
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Upgrade {equipment.Name}";
            PrintRich.PrintError(className, message);
        }
    }
    // * END - Class Methods

    // * START - Equipment Base Value Methods
    public static int GetWeaponAttack(Weapon weapon)
    {
        string fileName = "WeaponAttack";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> attackData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Array<GC.Dictionary<string, Variant>> attackDictionaries = attackData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();

        int attack = GetWeaponValue(attackDictionaries, weapon);
        return attack;
    }

    public static int GetWeaponSpecialAttack(Weapon weapon)
    {
        string fileName = "WeaponSpecial";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> specialData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Array<GC.Dictionary<string, Variant>> specialDictionaries = specialData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();

        int specialAttack = GetWeaponValue(specialDictionaries, weapon);
        return specialAttack;
    }
    
    private static int GetWeaponValue(GC.Array<GC.Dictionary<string, Variant>> dictionaries, Weapon weapon)
    {
        float value = 0;
        string treeName = weapon.Tree.ToString();

        foreach (GC.Dictionary<string, Variant> dictionary in dictionaries)
        {
            GC.Array<string> treeNames = dictionary["Trees"].As<GC.Array<string>>();
            if (!treeNames.Contains(treeName)) continue;

            value = dictionary["Value"].As<float>();
            float gradeIncrease = dictionary["GradeIncrease"].As<float>();
            float subGradeIncrease = dictionary["SubGradeIncrease"].As<float>();

            // Go through the grades first (Coarse tuning)
            for (int grade = 0; grade < weapon.Grade; grade++)
            {
                for (int subGrade = 0; subGrade < _MaxSubGrade; subGrade++)
                {
                    value += subGradeIncrease;
                }
                value += gradeIncrease;
            }

            // Go through sub grades afterwards (Fine tuning)
            for (int subGrade = 0; subGrade < weapon.SubGrade; subGrade++)
            {
                value += subGradeIncrease;
            }

            break;
        }
        return Mathf.RoundToInt(value);
    }

    public static int GetArmorDefense(int grade, int subGrade = 0)
    {
        string fileName = "ArmorDefense";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> defenseData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Array<GC.Array<int>> grades = defenseData[fileName].As<GC.Array<GC.Array<int>>>();

        GC.Array<int> defenseValues = grades[grade];
        int defenseValue = defenseValues[subGrade];

        return defenseValue;
    }
    // * END - Equipment Base Value Methods

    // * START - Equipment Value Calculation Methods
    public static int GetWeaponAttack()
    {
        int attack = Hunter.Weapon.Attack;

        (float Min, float Max) weaponPercentage = GetWeaponPercentage();

        RandomNumberGenerator RNG = new RandomNumberGenerator();
        float randomPercentage = RNG.RandfRange(weaponPercentage.Min, weaponPercentage.Max);

        return Mathf.RoundToInt(attack * randomPercentage);
    }

    public static int GetWeaponSpecialAttack()
    {
        if (Hunter.Weapon.Special == SpecialType.None) return 0;

        int specialAttack = Hunter.Weapon.SpecialAttack;

        (float Min, float Max) weaponPercentage = GetWeaponPercentage();

        RandomNumberGenerator RNG = new RandomNumberGenerator();
        float randomPercentage = RNG.RandfRange(weaponPercentage.Min, weaponPercentage.Max);

        return Mathf.RoundToInt(specialAttack * randomPercentage);
    }

    private static (float Min, float Max) GetWeaponPercentage() => Hunter.Weapon.Category switch
    {
        WeaponCategory.SwordAndShield => (0.05f, 0.15f),
        WeaponCategory.GreatSword => (0.4f, 0.5f),
        WeaponCategory.LongSword => (0.15f, 0.3f),
        _ => (0.05f, 0.25f)
    };

    public static float GetWeaponChargeTime() => Hunter.Weapon.Category switch
    {
        WeaponCategory.SwordAndShield => 0.45f,
        WeaponCategory.GreatSword => 1f,
        WeaponCategory.LongSword => 0.6f,
        _ => 0.5f
    };
    // * END - Equipment Value Calculation Methods

    // * START - Recipe Methods
    public static List<GC.Dictionary<string, Variant>> GetEquipmentRecipe(Equipment equipment)
    {
        // Load the file and get data
        string fileName = "EquipmentIngredients";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> equipmentIngredientsData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        List<GC.Dictionary<string, Variant>> ingredientDictionaries = equipmentIngredientsData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();

        // Find the dictionary based on the equipment group (tree/set)
        if (equipment is Weapon weapon)
        {
            GC.Dictionary<string, Variant> ingredientsDictionary = ingredientDictionaries.Find(dictionary => dictionary["Tree"].As<string>() == weapon.Tree.ToString());
            return GetNextWeaponRecipe(equipment, ingredientsDictionary);
        }
        else if (equipment is PalicoWeapon palicoWeapon)
        {
            GC.Dictionary<string, Variant> ingredientsDictionary = ingredientDictionaries.Find(dictionary => dictionary["Tree"].As<string>() == palicoWeapon.Tree.ToString());
            return GetNextWeaponRecipe(equipment, ingredientsDictionary);
        }
        else if (equipment is Armor armor)
        {
            GC.Dictionary<string, Variant> ingredientsDictionary = ingredientDictionaries.Find(dictionary => dictionary["Set"].As<string>() == armor.Set.ToString());
            return GetNextArmorRecipe(equipment, ingredientsDictionary);
        }
        else if (equipment is PalicoArmor palicoArmor)
        {
            GC.Dictionary<string, Variant> ingredientsDictionary = ingredientDictionaries.Find(dictionary => dictionary["Set"].As<string>() == palicoArmor.Set.ToString());
            return GetNextArmorRecipe(equipment, ingredientsDictionary);
        }
        else
        {
            // Print error
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Get Recipe For {equipment.Name}";
            string result = "Returning Null";
            PrintRich.PrintError(className, message, result);

            return null;
        }
    }

    private static List<GC.Dictionary<string, Variant>> GetNextWeaponRecipe(Equipment equipment, GC.Dictionary<string, Variant> ingredientsDictionary)
    {
        // Load the file and get data
        // Get the template for weapon recipes
        string fileName = "WeaponRecipe";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> weaponRecipeData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Dictionary<string, Variant> weaponRecipeTemplates = weaponRecipeData[fileName].As<GC.Dictionary<string, Variant>>();

        List<GC.Dictionary<string, Variant>> weaponRecipe = GetNextRecipe(equipment, ingredientsDictionary, weaponRecipeTemplates);
        return weaponRecipe;
    }

    private static List<GC.Dictionary<string, Variant>> GetNextArmorRecipe(Equipment equipment, GC.Dictionary<string, Variant> craftingDictionary)
    {
        // Load the file and get data
        // Get the template for armor recipes
        string fileName = "ArmorRecipe";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> armorRecipeData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Dictionary<string, Variant> armorRecipeTemplates = armorRecipeData[fileName].As<GC.Dictionary<string, Variant>>();

        List<GC.Dictionary<string, Variant>> armorRecipe = GetNextRecipe(equipment, craftingDictionary, armorRecipeTemplates);
        return armorRecipe;
    }

    private static List<Variant> GetRecipeTemplate(GC.Dictionary<string, Variant> ingredientsDictionary, GC.Dictionary<string, Variant> recipeTemplates)
    {
        try
        {
            // Get the specified recipes
            string monsterString = ingredientsDictionary["Monster"].As<string>();
            monsterString = monsterString != "None" ? "Monster" : "None";
            List<Variant> recipeTemplate = recipeTemplates[monsterString].As<GC.Array<Variant>>().ToList();

            return recipeTemplate;
        }
        catch
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Ingredients For Recipe Don't Exist";
            string result = "Returning Null";
            PrintRich.PrintError(className, message, result);

            return null;
        }
    }

    private static List<GC.Dictionary<string, Variant>> GetIngredientTemplates(Equipment equipment, List<Variant> recipeTemplate)
    {
        bool hasCrafted = HasCrafted(equipment);
        int grade = equipment.Grade;
        int subGrade = hasCrafted ? equipment.SubGrade + 1 : equipment.SubGrade;

        // Move onto the next grade
        if (subGrade > _MaxSubGrade && grade < _MaxGrade)
        {
            grade++;
            subGrade = 0;
        }

        List<Variant> subGrades = recipeTemplate[grade].As<GC.Array<Variant>>().ToList();
        if (subGrades.Count - 1 < subGrade)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"{equipment.Name} Is Maxed";
            string result = "Returning Null";
            PrintRich.PrintError(className, message, result);

            return null;
        }

        List<GC.Dictionary<string, Variant>> ingredientTemplates = subGrades[subGrade].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();

        return ingredientTemplates;
    }

    private static List<GC.Dictionary<string, Variant>> GetNextRecipe(Equipment equipment, GC.Dictionary<string, Variant> ingredientsDictionary, GC.Dictionary<string, Variant> recipeTemplates)
    {
        List<Variant> recipeTemplate = GetRecipeTemplate(ingredientsDictionary, recipeTemplates);
        List<GC.Dictionary<string, Variant>> ingredientTemplates = GetIngredientTemplates(equipment, recipeTemplate);

        if (ingredientTemplates == null) return null;

        // Create the recipe 
        List<GC.Dictionary<string, Variant>> recipe = new List<GC.Dictionary<string, Variant>>();

        EquipmentType equipmentType = EquipmentType.None;
        if (equipment is Weapon)
        {
            equipmentType = EquipmentType.Weapon;
        }
        else if (equipment is Armor)
        {
            equipmentType = EquipmentType.Armor;
        }

        foreach (GC.Dictionary<string, Variant> ingredientTemplate in ingredientTemplates)
        {
            int amount = ingredientTemplate["Amount"].As<int>();
            string ingredientName = GetIngredientName(ingredientTemplate, ingredientsDictionary, equipmentType);

            if (string.IsNullOrEmpty(ingredientName)) continue;

            GC.Dictionary<string, Variant> dictionary = new GC.Dictionary<string, Variant>()
            {
                { "Name", ingredientName },
                { "Amount", amount }
            };
            recipe.Add(dictionary);
        }
        return recipe;
    }

    // Get the name of the ingredient depending on the key's value through the dictionary of ingredients
    public static string GetIngredientName(GC.Dictionary<string, Variant> ingredientTemplate, GC.Dictionary<string, Variant> ingredientsDictionary, EquipmentType equipmentType, [CallerLineNumber] int lineNumber = 0)
    {
        string key = ingredientTemplate["Key"].As<string>();
        if (key == "EquipmentMaterial")
        {
            string materialName = equipmentType switch
            {
                EquipmentType.Weapon => "Sharp Claw",
                EquipmentType.Armor => "Wingdrake Hide",
                _ => ""
            };
            LocaleMaterial equipmentMaterial = LocaleManager.FindMaterial(materialName);
            return equipmentMaterial.Name;
        }

        try
        {
            string craftingKey = ingredientsDictionary[key].As<string>();
            string equipmentTypeString = $"{equipmentType}Material";

            bool hasEquipmentTypeString = ingredientsDictionary.Keys.Contains(equipmentTypeString);
            string materialTypeName = hasEquipmentTypeString ? ingredientsDictionary[equipmentTypeString].As<string>() : "";

            string ingredientName = key switch
            {
                "LocaleMaterial" => GetLocaleMaterialName(ingredientTemplate, craftingKey),
                "SubLocaleMaterial" => GetSubLocaleMaterialName(craftingKey),
                "Monster" => GetMonsterMaterialName(ingredientTemplate, materialTypeName, craftingKey),
                _ => GetMaterialName(craftingKey),
            };

            return ingredientName;

        }
        catch
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Ingredient Doesn't Exist - {key}";
            string result = $"Returning Empty String";
            PrintRich.PrintError(className, message, result);

            return "";
        }
    }

    // Convert the key + rarity into the desired ingredient | e.g. (Locale Material = Swamp) + (Rarity = 2) = Machalite Ore 
    private static string GetLocaleMaterialName(GC.Dictionary<string, Variant> ingredientTemplate, string localeName)
    {
        LocaleType localeType = Enum.Parse<LocaleType>(localeName);
        int rarity = ingredientTemplate["Rarity"].As<int>();
        LocaleMaterial localeMaterial = LocaleManager.GetLocaleMaterial(localeType, rarity);
        return localeMaterial.Name;
    }

    private static string GetSubLocaleMaterialName(string materialName)
    {
        LocaleMaterial subLocaleMaterial = LocaleManager.FindMaterial(materialName);
        return subLocaleMaterial.Name;
    }

    private static string GetMonsterMaterialName(GC.Dictionary<string, Variant> ingredientTemplate, string materialTypeName, string monsterName)
    {
        Monster monster = MonsterManager.FindMonster(monsterName);
        int rarity = ingredientTemplate["Rarity"].As<int>();

        List<MonsterMaterial> monsterMaterials = MonsterManager.GetMonsterMaterials(monster);
        if (rarity == 0)
        {
            List<MonsterMaterial> rarityOneMonsterMaterials = monsterMaterials.FindAll(material => material.Rarity == 0);
            MonsterMaterial monsterMaterial = rarityOneMonsterMaterials.Find(material => material.Name.Contains(materialTypeName));
            return monsterMaterial.Name;
        }
        else
        {
            MonsterMaterial monsterMaterial = monsterMaterials.Find(material => material.Rarity == rarity);
            return monsterMaterial.Name;
        }
    }

    private static string GetMaterialName(string materialName)
    {
        Material material = MonsterHunterIdle.FindMaterial(materialName);
        return material.Name;
    }
    // * END - Recipe Methods

    // * START - Data Methods
    /// <see cref="GameManager.SaveGame"/>
    public static GC.Dictionary<string, Variant> GetData()
    {
        GC.Dictionary<string, Variant> equipmentData = new GC.Dictionary<string, Variant>()
        {
            { "Weapons", GetWeaponsData() },
            { "Armor", GetArmorData() }
        };
        return equipmentData;
    }

    private static GC.Array<GC.Dictionary<string, Variant>> GetWeaponsData()
    {
        GC.Array<GC.Dictionary<string, Variant>> weaponsData = new GC.Array<GC.Dictionary<string, Variant>>();
        foreach (Weapon weapon in CraftedWeapons)
        {
            GC.Dictionary<string, Variant> weaponData = GetWeaponData(weapon);
            weaponsData.Add(weaponData);
        }
        return weaponsData;
    }

    public static GC.Dictionary<string, Variant> GetWeaponData(Weapon weapon)
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

    private static GC.Array<GC.Dictionary<string, Variant>> GetArmorData()
    {
        GC.Array<GC.Dictionary<string, Variant>> armorData = new GC.Array<GC.Dictionary<string, Variant>>();
        foreach (Armor armor in CraftedArmor)
        {
            GC.Dictionary<string, Variant> armorPieceData = GetArmorPieceData(armor);
            armorData.Add(armorPieceData);
        }
        return armorData;
    }

    public static GC.Dictionary<string, Variant> GetArmorPieceData(Armor armor)
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
    public static void SetData(GC.Dictionary<string, Variant> equipmentData)
    {
        CraftedWeapons.AddRange(GetWeaponsFromData(equipmentData));
        CraftedArmor.AddRange(GetArmorFromData(equipmentData));
    }

    private static List<Weapon> GetWeaponsFromData(GC.Dictionary<string, Variant> equipmentData)
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

    public static Weapon GetWeaponFromData(GC.Dictionary<string, Variant> weaponData)
    {
        WeaponCategory category = (WeaponCategory)weaponData["Category"].As<int>();
        WeaponTree tree = (WeaponTree)weaponData["Tree"].As<int>();
        int grade = weaponData["Grade"].As<int>();
        int subGrade = weaponData["SubGrade"].As<int>();

        Weapon weapon = GetWeapon(category, tree, grade, subGrade);
        return weapon;
    }

    private static List<Armor> GetArmorFromData(GC.Dictionary<string, Variant> equipmentData)
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

    public static Armor GetArmorPieceFromData(GC.Dictionary<string, Variant> armorData)
    {
        ArmorCategory category = (ArmorCategory)armorData["Category"].As<int>();
        ArmorSet set = (ArmorSet)armorData["Set"].As<int>();
        int grade = armorData["Grade"].As<int>();
        int subGrade = armorData["SubGrade"].As<int>();

        Armor armorPiece = GetArmor(category, set, grade, subGrade);
        return armorPiece;
    }

    /// <see cref="GameManager.DeleteGame"/>
    public static void DeleteData()
    {
        Weapons.Clear();
        Armor.Clear();

        CraftedWeapons.Clear();
        CraftedArmor.Clear();

        LoadEquipment();
    }
    // * END - Data Methods
}