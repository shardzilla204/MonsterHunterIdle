using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class PalicoEquipmentManager : Node
{
    [Export]
    private bool _hasStartingEquipment = false;

    [Export]
    private GC.Array<GC.Dictionary<string, Variant>> _startingEquipment = new GC.Array<GC.Dictionary<string, Variant>>()
    {
        new GC.Dictionary<string, Variant>()
        {
            { "Type", (int) PalicoEquipmentType.Weapon },
            { "Tree", (int) WeaponTree.Ore },
            { "Grade", 0 },
            { "SubGrade", 0 }
        },
        new GC.Dictionary<string, Variant>()
        {
            { "Type", (int) PalicoEquipmentType.Weapon },
            { "Tree", (int) WeaponTree.Ore },
            { "Grade", 2 },
            { "SubGrade", 0 }
        },
        new GC.Dictionary<string, Variant>()
        {
            { "Type", (int) PalicoEquipmentType.Head },
            { "Set", (int) ArmorSet.Leather },
            { "Grade", 0 },
            { "SubGrade", 0 }
        },
        new GC.Dictionary<string, Variant>()
        {
            { "Type", (int) PalicoEquipmentType.Chest },
            { "Set", (int) ArmorSet.Leather },
            { "Grade", 0 },
            { "SubGrade", 0 }
        }
    };
    public static List<PalicoWeapon> Weapons = new List<PalicoWeapon>();
    public static List<PalicoArmor> Armor = new List<PalicoArmor>();

    public static GC.Dictionary<string, Variant> Recipes = new GC.Dictionary<string, Variant>();

    public static List<PalicoWeapon> CraftedWeapons = new List<PalicoWeapon>();
    public static List<PalicoArmor> CraftedArmor = new List<PalicoArmor>();

    public static bool HasStartingEquipment = false;
    public static GC.Array<GC.Dictionary<string, Variant>> StartingEquipment = new GC.Array<GC.Dictionary<string, Variant>>();

    private const int _MaxGrade = 4;
    private const int _MaxSubGrade = 4;

    public override void _EnterTree()
    {
        LoadPalicoWeapons();
        LoadPalicoArmor();

        HasStartingEquipment = _hasStartingEquipment;
        StartingEquipment = _startingEquipment;
    }

    // * START - File Methods
    private static void LoadPalicoWeapons()
    {
        string fileName = "PalicoWeapons";
        string folderPath = "Equipment/Palico";

        GC.Dictionary<string, Variant> palicoWeaponsData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
        if (palicoWeaponsData == null) return;

        GC.Dictionary<string, Variant> palicoWeaponDictionaries = palicoWeaponsData[fileName].As<GC.Dictionary<string, Variant>>();
        foreach (string treeName in palicoWeaponDictionaries.Keys)
        {
            GC.Dictionary<string, Variant> weaponTreeDictionary = palicoWeaponDictionaries[treeName].As<GC.Dictionary<string, Variant>>();
            string treeNameString = treeName.Replace(" ", "");
            treeNameString = treeNameString.Replace("-", "");
            WeaponTree tree = Enum.Parse<WeaponTree>(treeNameString);

            PalicoWeapon palicoWeapon = new PalicoWeapon(tree);
            palicoWeapon.SetEquipment(weaponTreeDictionary);

            Weapons.Add(palicoWeapon);
        }
    }

    private static void LoadPalicoArmor()
    {
        string fileName = "PalicoArmor";
        string folderPath = "Equipment/Palico";

        GC.Dictionary<string, Variant> palicoArmorData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
        if (palicoArmorData == null) return;

        GC.Dictionary<string, Variant> setDictionaries = palicoArmorData[fileName].As<GC.Dictionary<string, Variant>>();
        foreach (string setName in setDictionaries.Keys)
        {
            ArmorSet set = Enum.Parse<ArmorSet>(setName);
            GC.Array<string> setDictionary = setDictionaries[setName].As<GC.Array<string>>();

            /// Start at 1 for just armor | <see cref="PalicoEquipmentType">
            for (int typeIndex = 1; typeIndex <= setDictionary.Count; typeIndex++)
            {
                PalicoEquipmentType type = (PalicoEquipmentType)typeIndex;
                PalicoArmor palicoArmor = new PalicoArmor(type, set);
                palicoArmor.SetEquipment(setDictionaries);

                Armor.Add(palicoArmor);
            }
        }
    }
    // * END - File Methods

    public static void AddStartingEquipment()
    {
        foreach (GC.Dictionary<string, Variant> equipmentDictionary in StartingEquipment)
        {
            PalicoEquipmentType equipmentType = (PalicoEquipmentType)equipmentDictionary["Type"].As<int>();
            int grade = equipmentDictionary["Grade"].As<int>();
            int subGrade = equipmentDictionary["SubGrade"].As<int>();

            if (equipmentType == PalicoEquipmentType.Weapon)
            {
                WeaponTree tree = (WeaponTree)equipmentDictionary["Tree"].As<int>();
                AddStartingWeapon(tree, grade, subGrade);
            }
            else
            {
                ArmorSet set = (ArmorSet)equipmentDictionary["Set"].As<int>();
                AddStartingArmor(equipmentType, set, grade, subGrade);
            }
        }
    }

    public static void AddStartingWeapon(WeaponTree tree, int grade, int subGrade)
    {
        PalicoWeapon weapon = GetWeapon(tree, grade, subGrade);
        if (weapon == null) return;

        CraftedWeapons.Add(weapon);
    }

    public static void AddStartingArmor(PalicoEquipmentType equipmentType, ArmorSet set, int grade, int subGrade)
    {
        PalicoArmor armor = GetArmor(equipmentType, set, grade, subGrade);
        if (armor == null) return;

        CraftedArmor.Add(armor);
    }

    public static PalicoWeapon GetWeapon(WeaponTree tree, int grade = 0, int subGrade = 0)
    {
        if (tree == WeaponTree.None) return new PalicoWeapon();

        string fileName = "PalicoWeapons";
        string folderPath = "Equipment/Palico";
        GC.Dictionary<string, Variant> weaponTreeData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
        if (weaponTreeData == null) return null;

        GC.Dictionary<string, Variant> weaponTreeDictionaries = weaponTreeData[fileName].As<GC.Dictionary<string, Variant>>();

        string treeString = MonsterHunterIdle.AddSpacing(tree.ToString());

        GC.Dictionary<string, Variant> weaponGradeDictionary = weaponTreeDictionaries[treeString].As<GC.Dictionary<string, Variant>>();

        PalicoWeapon weapon = new PalicoWeapon(tree);
        weapon.Grade = grade;
        weapon.SubGrade = subGrade;
        weapon.SetEquipment(weaponGradeDictionary);

        return weapon;
    }

    public static PalicoArmor GetArmor(PalicoEquipmentType type, ArmorSet set, int grade = 0, int subGrade = 0)
    {
        if (set == ArmorSet.None) return new PalicoArmor();

        string fileName = "PalicoArmor";
        string folderPath = "Equipment/Palico";
        GC.Dictionary<string, Variant> armorData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
        if (armorData == null) return null;

        GC.Dictionary<string, Variant> setDictionaries = armorData[fileName].As<GC.Dictionary<string, Variant>>();

        PalicoArmor armor = new PalicoArmor(type, set);
        armor.Grade = grade;
        armor.SubGrade = subGrade;
        armor.SetEquipment(setDictionaries);

        return armor;
    }

    public static string GetWeaponName(PalicoWeapon weapon, string weaponName)
    {
        string romanNumeral = MonsterHunterIdle.GetRomanNumeral(weapon.Grade);
        return $"{weaponName} {romanNumeral}";
    }

    public static string GetArmorName(PalicoArmor armor, string armorName)
    {
        string romanNumeral = MonsterHunterIdle.GetRomanNumeral(armor.Grade);
        return $"Felyne {armorName} {romanNumeral}";
    }

    public static int GetAttackValue(PalicoWeapon weapon)
    {
        string fileName = "PalicoWeaponAttack";
        string folderPath = "Equipment/Palico";
        GC.Dictionary<string, Variant> attackData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
        GC.Array<GC.Dictionary<string, Variant>> attackDictionaries = attackData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();

        int attack = GetWeaponValue(attackDictionaries, weapon);
        return attack;
    }

    public static int GetSpecialValue(PalicoWeapon weapon)
    {
        string fileName = "PalicoWeaponSpecial";
        string folderPath = "Equipment/Palico";
        GC.Dictionary<string, Variant> specialData = MonsterHunterIdle.LoadFile(fileName, folderPath).As<GC.Dictionary<string, Variant>>();
        GC.Array<GC.Dictionary<string, Variant>> specialDictionaries = specialData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();

        int special = GetWeaponValue(specialDictionaries, weapon);
        return special;
    }

    private static int GetWeaponValue(GC.Array<GC.Dictionary<string, Variant>> dictionaries, PalicoWeapon weapon)
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

    // * START - Equipment Value Calculation Methods
    public static int GetWeaponAttack(Palico palico)
    {
        int attack = palico.Weapon.Attack;

        float minPercentage = 0.25f;
        float maxPercentage = 1;

        RandomNumberGenerator RNG = new RandomNumberGenerator();
        float randomPercentage = RNG.RandfRange(minPercentage, maxPercentage);

        return Mathf.RoundToInt(attack * randomPercentage);
    }

    public static int GetWeaponSpecialAttack(Palico palico)
    {
        if (palico.Weapon.Special == SpecialType.None) return 0;

        int specialAttack = palico.Weapon.SpecialAttack;

        float minPercentage = 0.25f;
        float maxPercentage = 1;

        RandomNumberGenerator RNG = new RandomNumberGenerator();
        float randomPercentage = RNG.RandfRange(minPercentage, maxPercentage);

        return Mathf.RoundToInt(specialAttack * randomPercentage);
    }
    // * END - Equipment Value Calculation Methods

    public static int GetCraftingCost(int grade, int subGrade)
    {
        if (subGrade > _MaxSubGrade && grade < _MaxGrade)
        {
            grade++;
            subGrade = 0;
        }

        int craftingCost = MonsterHunterIdle.GetCraftingCost(grade, subGrade);
        return craftingCost;
    }

    public static void UpgradeEquipment(PalicoEquipment equipment)
    {
        equipment.SubGrade++;

        if (equipment.SubGrade > _MaxSubGrade && equipment.Grade < _MaxGrade)
        {
            equipment.Grade++;
            equipment.SubGrade = 0;
        }

        // Set the details of the equipment
        if (equipment is PalicoWeapon weapon)
        {
            PalicoWeapon upgradedWeapon = GetWeapon(weapon.Tree, weapon.Grade, weapon.SubGrade);
            weapon.Name = upgradedWeapon.Name;
            weapon.Attack = upgradedWeapon.Attack;
            weapon.SpecialAttack = upgradedWeapon.SpecialAttack;
            weapon.Affinity = upgradedWeapon.Affinity;
        }
        else if (equipment is PalicoArmor armor)
        {
            PalicoArmor upgradedArmor = GetArmor(armor.Type, armor.Set, armor.Grade, armor.SubGrade);
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

    public static PalicoWeapon FindCraftedWeapon(PalicoWeapon targetWeapon)
    {
        return CraftedWeapons.Find(weapon => weapon.Tree == targetWeapon.Tree);
    }

    public static PalicoArmor FindCraftedArmorPiece(PalicoArmor targetArmor)
    {
        return CraftedArmor.Find(armor => armor.Type == targetArmor.Type && armor.Set == targetArmor.Set);
    }

    public static PalicoWeapon FindWeapon(PalicoWeapon targetWeapon)
    {
        return Weapons.Find(weapon => weapon.Tree == targetWeapon.Tree);
    }

    public static PalicoArmor FindArmorPiece(PalicoArmor targetArmor)
    {
        return Armor.Find(armor => armor.Type == targetArmor.Type && armor.Set == targetArmor.Set);
    }

    public static List<PalicoWeapon> FindCraftedWeapons(PalicoWeapon targetWeapon)
    {
        return CraftedWeapons.FindAll(weapon => weapon.Tree == targetWeapon.Tree);
    }

    public static List<PalicoArmor> FindCraftedArmor(PalicoArmor targetArmor)
    {
        return CraftedArmor.FindAll(armor => armor.Type == targetArmor.Type && armor.Set == targetArmor.Set);
    }

    public static int GetEquipmentAmount(PalicoEquipment equipment)
    {
        if (equipment is PalicoWeapon targetWeapon)
        {
            int amount = FindCraftedWeapons(targetWeapon).Count;
            return amount;
        }
        else if (equipment is PalicoArmor targetArmor)
        {
            int amount = FindCraftedArmor(targetArmor).Count;
            return amount;
        }
        else
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Get Equipment Amount For {equipment.Name}";
            string result = "Returning 0";
            PrintRich.PrintError(className, message, result);

            return 0;
        }
    }

    public static PalicoEquipment FindEquipmentPiece(PalicoEquipment equipment)
    {
        if (equipment is PalicoWeapon targetWeapon)
        {
            PalicoWeapon weapon = FindWeapon(targetWeapon);
            return weapon;
        }
        else if (equipment is PalicoArmor targetArmor)
        {
            PalicoArmor armorPiece = FindArmorPiece(targetArmor);
            return armorPiece;
        }
        else
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Find Equipment: {equipment.Name}";
            string result = "Returning Null";
            PrintRich.PrintError(className, message, result);

            return null;
        }
    }

    public static PalicoEquipment FindCraftedEquipmentPiece(PalicoEquipment equipment)
    {
        if (equipment is PalicoWeapon targetWeapon)
        {
            PalicoWeapon weapon = FindCraftedWeapon(targetWeapon);
            return weapon;
        }
        else if (equipment is PalicoArmor targetArmor)
        {
            PalicoArmor armorPiece = FindCraftedArmorPiece(targetArmor);
            return armorPiece;
        }
        else
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Find Equipment: {equipment.Name}";
            string result = "Returning Null";
            PrintRich.PrintError(className, message, result);

            return null;
        }
    }

    public static List<PalicoEquipment> FindCraftedEquipment(Equipment equipment)
    {
        List<PalicoEquipment> equipmentPieces = new List<PalicoEquipment>();
        if (equipment is PalicoWeapon targetWeapon)
        {
            List<PalicoWeapon> weapons = FindCraftedWeapons(targetWeapon);
            equipmentPieces = [.. weapons];
        }
        else if (equipment is PalicoArmor targetArmor)
        {
            List<PalicoArmor> armor = FindCraftedArmor(targetArmor);
            equipmentPieces = [.. armor];
        }
        return equipmentPieces;
    }

    // See if there are more Palicos than the current amount of equipment
    public static bool CanCraft(Equipment equipment)
    {
        List<PalicoEquipment> equipmentPieces = FindCraftedEquipment(equipment);
        return PalicoManager.Palicos.Count > equipmentPieces.Count;
    }

    public static List<GC.Dictionary<string, Variant>> GetEquipmentRecipe(PalicoEquipment equipment, bool isGettingNextRecipe)
    {
        // Load the file and get data
        string fileName = "EquipmentIngredients";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> equipmentIngredientsData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        List<GC.Dictionary<string, Variant>> ingredientDictionaries = equipmentIngredientsData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();

        // Find the dictionary based on the equipment group (tree/set)
        if (equipment is PalicoWeapon palicoWeapon)
        {
            GC.Dictionary<string, Variant> ingredientsDictionary = ingredientDictionaries.Find(dictionary => dictionary["Tree"].As<string>() == palicoWeapon.Tree.ToString());
            return GetNextWeaponRecipe(equipment, isGettingNextRecipe, ingredientsDictionary);
        }
        else if (equipment is PalicoArmor palicoArmor)
        {
            GC.Dictionary<string, Variant> ingredientsDictionary = ingredientDictionaries.Find(dictionary => dictionary["Set"].As<string>() == palicoArmor.Set.ToString());
            return GetNextArmorRecipe(equipment, isGettingNextRecipe, ingredientsDictionary);
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

    private static List<GC.Dictionary<string, Variant>> GetNextWeaponRecipe(PalicoEquipment equipment, bool isGettingNextRecipe, GC.Dictionary<string, Variant> ingredientsDictionary)
    {
        // Load the file and get data
        // Get the template for weapon recipes
        string fileName = "WeaponRecipe";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> weaponRecipeData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Dictionary<string, Variant> weaponRecipeTemplates = weaponRecipeData[fileName].As<GC.Dictionary<string, Variant>>();

        List<GC.Dictionary<string, Variant>> weaponRecipe = GetNextRecipe(equipment, isGettingNextRecipe, ingredientsDictionary, weaponRecipeTemplates);
        return weaponRecipe;
    }

    private static List<GC.Dictionary<string, Variant>> GetNextArmorRecipe(PalicoEquipment equipment, bool isGettingNextRecipe, GC.Dictionary<string, Variant> craftingDictionary)
    {
        // Load the file and get data
        // Get the template for armor recipes
        string fileName = "ArmorRecipe";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> armorRecipeData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Dictionary<string, Variant> armorRecipeTemplates = armorRecipeData[fileName].As<GC.Dictionary<string, Variant>>();

        List<GC.Dictionary<string, Variant>> armorRecipe = GetNextRecipe(equipment, isGettingNextRecipe, craftingDictionary, armorRecipeTemplates);
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

    private static List<GC.Dictionary<string, Variant>> GetIngredientTemplates(PalicoEquipment equipment, bool isGettingNextRecipe, List<Variant> recipeTemplate)
    {
        int grade = equipment.Grade;
        int subGrade = isGettingNextRecipe ? equipment.SubGrade + 1 : equipment.SubGrade;

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

    private static List<GC.Dictionary<string, Variant>> GetNextRecipe(PalicoEquipment equipment, bool isGettingNextRecipe, GC.Dictionary<string, Variant> ingredientsDictionary, GC.Dictionary<string, Variant> recipeTemplates)
    {
        List<Variant> recipeTemplate = GetRecipeTemplate(ingredientsDictionary, recipeTemplates);
        List<GC.Dictionary<string, Variant>> ingredientTemplates = GetIngredientTemplates(equipment, isGettingNextRecipe, recipeTemplate);

        if (ingredientTemplates == null) return null;

        // Create the recipe 
        List<GC.Dictionary<string, Variant>> recipe = new List<GC.Dictionary<string, Variant>>();

        EquipmentType equipmentType = EquipmentType.None;
        if (equipment is PalicoWeapon)
        {
            equipmentType = EquipmentType.Weapon;
        }
        else if (equipment is PalicoArmor)
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
        foreach (PalicoWeapon weapon in CraftedWeapons)
        {
            GC.Dictionary<string, Variant> weaponData = GetWeaponData(weapon);
            weaponsData.Add(weaponData);
        }
        return weaponsData;
    }

    public static GC.Dictionary<string, Variant> GetWeaponData(PalicoWeapon weapon)
    {
        GC.Dictionary<string, Variant> weaponData = new GC.Dictionary<string, Variant>()
        {
            { "Type", (int) weapon.Type },
            { "Tree", (int) weapon.Tree },
            { "Grade", weapon.Grade },
            { "SubGrade", weapon.SubGrade }
        };
        return weaponData;
    }

    private static GC.Array<GC.Dictionary<string, Variant>> GetArmorData()
    {
        GC.Array<GC.Dictionary<string, Variant>> armorData = new GC.Array<GC.Dictionary<string, Variant>>();
        foreach (PalicoArmor armor in CraftedArmor)
        {
            GC.Dictionary<string, Variant> armorPieceData = GetArmorPieceData(armor);
            armorData.Add(armorPieceData);
        }
        return armorData;
    }

    public static GC.Dictionary<string, Variant> GetArmorPieceData(PalicoArmor armor)
    {
        GC.Dictionary<string, Variant> armorPieceData = new GC.Dictionary<string, Variant>()
        {
            { "Type", (int) armor.Type },
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

    private static List<PalicoWeapon> GetWeaponsFromData(GC.Dictionary<string, Variant> equipmentData)
    {
        List<PalicoWeapon> weapons = new List<PalicoWeapon>();
        GC.Array<GC.Dictionary<string, Variant>> weaponsData = equipmentData["Weapons"].As<GC.Array<GC.Dictionary<string, Variant>>>();
        foreach (GC.Dictionary<string, Variant> weaponData in weaponsData)
        {
            PalicoWeapon weapon = GetWeaponFromData(weaponData);
            weapons.Add(weapon);
        }
        return weapons;
    }

    public static PalicoWeapon GetWeaponFromData(GC.Dictionary<string, Variant> weaponData)
    {
        WeaponTree tree = (WeaponTree)weaponData["Tree"].As<int>();
        int grade = weaponData["Grade"].As<int>();
        int subGrade = weaponData["SubGrade"].As<int>();

        PalicoWeapon weapon = GetWeapon(tree, grade, subGrade);
        return weapon;
    }

    private static List<PalicoArmor> GetArmorFromData(GC.Dictionary<string, Variant> equipmentData)
    {
        List<PalicoArmor> armor = new List<PalicoArmor>();
        GC.Array<GC.Dictionary<string, Variant>> armorData = equipmentData["Armor"].As<GC.Array<GC.Dictionary<string, Variant>>>();
        foreach (GC.Dictionary<string, Variant> armorPieceData in armorData)
        {
            PalicoArmor armorPiece = GetArmorPieceFromData(armorPieceData);
            armor.Add(armorPiece);
        }
        return armor;
    }

    public static PalicoArmor GetArmorPieceFromData(GC.Dictionary<string, Variant> armorData)
    {
        PalicoEquipmentType type = (PalicoEquipmentType)armorData["Type"].As<int>();
        ArmorSet set = (ArmorSet)armorData["Set"].As<int>();
        int grade = armorData["Grade"].As<int>();
        int subGrade = armorData["SubGrade"].As<int>();

        PalicoArmor armorPiece = GetArmor(type, set, grade, subGrade);
        return armorPiece;
    }

    /// <see cref="GameManager.DeleteGame"/>
    public static void DeleteData()
    {
        Weapons.Clear();
        Armor.Clear();

        CraftedWeapons.Clear();
        CraftedArmor.Clear();

        LoadPalicoWeapons();
        LoadPalicoArmor();

        if (HasStartingEquipment) AddStartingEquipment();
    }
    // * END - Data Methods
}