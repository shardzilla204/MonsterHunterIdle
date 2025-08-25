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

    public override void _EnterTree()
    {
        MonsterHunterIdle.EquipmentManager = this;

        LoadEquipment();
    }

    // Go through provided file paths and load weapons and armor
    private void LoadEquipment()
    {
        string weaponFilePath = $"res://JSON/{_weaponFolderPath}/";
        LoadWeapons(weaponFilePath);

        LoadArmor();
    }

    /// Iterate through the provided file path e.g "res://JSON/Equipment/Weapons/".
    /// Take the file name without the extension (<see cref="string.GetBaseName()"/>) 
    /// Use fileName and folderPath (<see cref="LoadWeapons.folderPath"/>) as parameters in the provided method
    /// If successful, iterate to the next file
    private void LoadWeapons(string filePath)
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

                bool hasPassed = AddWeapons(fileName.GetBaseName());
                if (!hasPassed)
                {
                    string errorMessage = $"Couldn't Add Weapon Using The Path: {filePath}";
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

    private bool AddWeapons(string fileName)
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

    private void LoadArmor()
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
                ArmorCategory category = (ArmorCategory) categoryIndex;
                Armor armor = new Armor(category, set);
                armor.SetEquipment(setDictionaries);
                
                Armor.Add(armor);
            }
        }
    }

    public Armor GetArmor(ArmorCategory category, ArmorSet set, int grade, int subGrade)
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

    public void SetWeaponName(Weapon weapon, GC.Array<string> names)
    {
        int maxSubGrade = 4;
        int grade = weapon.Grade;
        string romanNumeral = GetRomanNumeral(grade);
        if (weapon.Grade < maxSubGrade)
        {
            weapon.Name = $"{names[0]} {romanNumeral}";
        }
        else
        {
            grade -= maxSubGrade;
            romanNumeral = GetRomanNumeral(grade);
            weapon.Name = $"{names[1]} {romanNumeral}";
        }
    }

    public void SetArmorName(Armor armor, GC.Array<string> names)
    {
        int armorCategoryIndex = (int) armor.Category;
        string romanNumeral = GetRomanNumeral(armor.Grade);
        armor.Name = $"{armor.Set} {names[armorCategoryIndex]} {romanNumeral}";
    }

    private string GetRomanNumeral(int grade) => grade switch
    {
        0 => "I",
        1 => "II",
        2 => "III",
        3 => "IV",
        4 => "V",
        5 => "VI",
        6 => "VII",
        7 => "VII",
        8 => "IX",
        9 => "X",
        _ => ""
    };

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

    public int GetCraftingCost(int grade, int subGrade)
    {
        int[,] craftingCosts =
        {
            { 10, 20, 30, 40, 50 },
            { 300, 100, 150, 200, 250 },
            { 600, 200, 300, 400, 500 },
            { 900, 300, 450, 600, 750 },
            { 1500, 500, 750, 1000, 1250 },
            { 3000, 1000, 1500, 2000, 2500 },
            { 6000, 2000, 3000, 4000, 5000 },
            { 12000, 4000, 6000, 8000, 10000 },
            { 30000, 10000, 15000, 20000, 25000 },
            { 75000, 25000, 37500, 50000, 62500 }
        };

        int maxSubGrade = 4;
        if (subGrade > maxSubGrade && grade < maxSubGrade)
        {
            grade++;
            subGrade = 0;
        }
        int craftingCost = craftingCosts[grade, subGrade];

        return craftingCost;
    }

    public void UpgradeEquipment(Equipment equipment)
    {
        equipment.SubGrade++;

        int maxSubGrade = 4;
        if (equipment.SubGrade > maxSubGrade && equipment.Grade < maxSubGrade)
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
            string errorMessage = $"Couldn't Upgrade {equipment.Name}";
            GD.PrintErr(errorMessage);
        }
    }

    public int GetAttackValue(Weapon weapon)
    {
        string fileName = "WeaponAttack";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> attackData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Array<GC.Dictionary<string, Variant>> attackDictionaries = attackData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();

        int attack = GetWeaponValue(attackDictionaries, weapon);
        return attack;
    }

    public int GetSpecialValue(Weapon weapon)
    {
        string fileName = "WeaponSpecial";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> specialData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Array<GC.Dictionary<string, Variant>> specialDictionaries = specialData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>();

        int special = GetWeaponValue(specialDictionaries, weapon);
        return special;
    }

    private int GetWeaponValue(GC.Array<GC.Dictionary<string, Variant>> dictionaries, Weapon weapon)
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
            int maxSubGrade = 5;
            for (int grade = 0; grade < weapon.Grade; grade++)
            {
                for (int subGrade = 0; subGrade < maxSubGrade; subGrade++)
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

    public List<GC.Dictionary<string, Variant>> GetRecipe(Equipment equipment, bool getNextRecipe = false)
    {
        // Get the specified ingredient names
        string fileName = "RecipeIngredients";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> craftingData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        List<GC.Dictionary<string, Variant>> craftingDictionaries = craftingData[fileName].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();

        if (equipment is Weapon weapon)
        {
            GC.Dictionary<string, Variant> craftingDictionary = craftingDictionaries.Find(dictionary => dictionary["Tree"].As<string>() == weapon.Tree.ToString());
            return GetWeaponRecipe(equipment, getNextRecipe, craftingDictionary);
        }
        else if (equipment is Armor armor)
        {
            GC.Dictionary<string, Variant> craftingDictionary = craftingDictionaries.Find(dictionary => dictionary["Set"].As<string>() == armor.Set.ToString());
            return GetArmorRecipe(equipment, getNextRecipe, craftingDictionary);
        }

        return null;
    }

    private List<GC.Dictionary<string, Variant>> GetArmorRecipe(Equipment equipment, bool getNextRecipe, GC.Dictionary<string, Variant> craftingDictionary)
    {
        // Get the dictionaries
        string fileName = "ArmorRecipe";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> armorRecipeData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Dictionary<string, Variant> armorRecipeDictionaries = armorRecipeData[fileName].As<GC.Dictionary<string, Variant>>();

        List<GC.Dictionary<string, Variant>> armorRecipe = GetRecipe(equipment, getNextRecipe, craftingDictionary, armorRecipeDictionaries);
        return armorRecipe;
    }

    private List<GC.Dictionary<string, Variant>> GetWeaponRecipe(Equipment equipment, bool getNextRecipe, GC.Dictionary<string, Variant> craftingDictionary)
    {
        // Get the dictionaries
        string fileName = "WeaponRecipe";
        string folderName = "Equipment";
        GC.Dictionary<string, Variant> weaponRecipeData = MonsterHunterIdle.LoadFile(fileName, folderName).As<GC.Dictionary<string, Variant>>();
        GC.Dictionary<string, Variant> weaponRecipeDictionaries = weaponRecipeData[fileName].As<GC.Dictionary<string, Variant>>();

        List<GC.Dictionary<string, Variant>> weaponRecipe = GetRecipe(equipment, getNextRecipe, craftingDictionary, weaponRecipeDictionaries);
        return weaponRecipe;
    }

    private List<Variant> GetGradeDictionaries(GC.Dictionary<string, Variant> craftingDictionary, GC.Dictionary<string, Variant> equipmentRecipeDictionaries)
    {
        try
        {
            // Get the specified grade dictionaries
            string monsterString = craftingDictionary["Monster"].As<string>();
            monsterString = monsterString != "None" ? "Monster" : "None";
            List<Variant> gradeDictionaries = equipmentRecipeDictionaries[monsterString].As<GC.Array<Variant>>().ToList();

            return gradeDictionaries;
        }
        catch
        {
            string errorMessage = $"Ingredients For Recipe Don't Exist";

            GD.PrintErr(errorMessage);

            return null;
        }
    }

    private List<GC.Dictionary<string, Variant>> GetIngredientDictionaries(Equipment equipment, bool getNextRecipe, List<Variant> gradeDictionaries)
    {
        int maxSubGrade = 4;
        int grade = equipment.Grade;
        int subGrade = getNextRecipe ? equipment.SubGrade + 1 : equipment.SubGrade;

        // Move onto the next grade
        if (subGrade > maxSubGrade && grade < maxSubGrade)
        {
            grade++;
            subGrade = 0;
        }

        List<Variant> subGradeDictionaries = gradeDictionaries[grade].As<GC.Array<Variant>>().ToList();
        List<GC.Dictionary<string, Variant>> ingredientDictionaries = subGradeDictionaries[subGrade].As<GC.Array<GC.Dictionary<string, Variant>>>().ToList();

        return ingredientDictionaries;
    }

    private List<GC.Dictionary<string, Variant>> GetRecipe(Equipment equipment, bool getNextRecipe, GC.Dictionary<string, Variant> craftingDictionary, GC.Dictionary<string, Variant> equipmentRecipeDictionaries)
    {
        List<Variant> gradeDictionaries = GetGradeDictionaries(craftingDictionary, equipmentRecipeDictionaries);
        List<GC.Dictionary<string, Variant>> ingredientDictionaries = GetIngredientDictionaries(equipment, getNextRecipe, gradeDictionaries);

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

        foreach (GC.Dictionary<string, Variant> ingredientDictionary in ingredientDictionaries)
        {
            int amount = ingredientDictionary["Amount"].As<int>();
            GC.Dictionary<string, Variant> dictionary = new GC.Dictionary<string, Variant>()
            {
                { "Name", GetIngredientName(ingredientDictionary, craftingDictionary, equipmentType) },
                { "Amount", amount }
            };
            recipe.Add(dictionary);
        }
        return recipe;
    }

    private string GetIngredientName(GC.Dictionary<string, Variant> ingredientDictionary, GC.Dictionary<string, Variant> craftingDictionary, EquipmentType equipmentType)
    {
        string key = ingredientDictionary["Key"].As<string>();
        if (key == "EquipmentMaterial")
        {
            string materialName = equipmentType switch
            {
                EquipmentType.Weapon => "Sharp Claw",
                EquipmentType.Armor => "Wingdrake Hide",
                _ => ""
            };
            LocaleMaterial equipmentMaterial = MonsterHunterIdle.LocaleManager.FindMaterial(materialName);
            return equipmentMaterial.Name;
        }

        string craftingKey = craftingDictionary[key].As<string>();
        string equipmentTypeString = $"{equipmentType}Material";

        bool hasEquipmentTypeString = craftingDictionary.Keys.Contains(equipmentTypeString);
        string materialTypeName = hasEquipmentTypeString ? craftingDictionary[equipmentTypeString].As<string>() : "";

        string ingredientName = key switch
        {
            "LocaleMaterial" => GetLocaleMaterialName(ingredientDictionary, craftingKey),
            "SubLocaleMaterial" => GetSubLocaleMaterialName(craftingKey),
            "Monster" => GetMonsterMaterialName(ingredientDictionary, materialTypeName, craftingKey),
            _ => GetMaterialName(craftingKey),
        };

        return ingredientName;
    }

    // Convert the key + rarity into the desired ingredient | e.g. (Locale Material = Swamp) + (Rarity = 2) = Machalite Ore 
    private string GetLocaleMaterialName(GC.Dictionary<string, Variant> ingredientDictionary, string localeName)
    {
        LocaleType localeType = Enum.Parse<LocaleType>(localeName);
        int rarity = ingredientDictionary["Rarity"].As<int>();
        LocaleMaterial localeMaterial = MonsterHunterIdle.LocaleManager.GetLocaleMaterial(localeType, rarity);
        return localeMaterial.Name;
    }

    private string GetSubLocaleMaterialName(string materialName)
    {
        LocaleMaterial subLocaleMaterial = MonsterHunterIdle.LocaleManager.FindMaterial(materialName);
        return subLocaleMaterial.Name;
    }

    private string GetMonsterMaterialName(GC.Dictionary<string, Variant> ingredientDictionary, string materialTypeName, string monsterName)
    {
        Monster monster = MonsterHunterIdle.MonsterManager.FindMonster(monsterName);
        int rarity = ingredientDictionary["Rarity"].As<int>();

        List<MonsterMaterial> monsterMaterials = MonsterHunterIdle.MonsterManager.GetMonsterMaterials(monster);
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

    private string GetMaterialName(string materialName)
    {
        Material material = MonsterHunterIdle.FindMaterial(materialName);
        return material.Name;
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
        WeaponCategory category = (WeaponCategory)weaponData["Category"].As<int>();
        WeaponTree tree = (WeaponTree)weaponData["Tree"].As<int>();
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
        ArmorCategory category = (ArmorCategory)armorData["Category"].As<int>();
        ArmorSet set = (ArmorSet)armorData["Set"].As<int>();
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