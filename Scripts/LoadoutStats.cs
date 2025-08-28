using Godot;

namespace MonsterHunterIdle;

public partial class LoadoutStats : NinePatchRect
{
    [Export]
    private Container _statContainer;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged -= EquipmentChanged;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged += EquipmentChanged;
    }

    public override void _Ready()
    {
        ClearStats();
        SetStats();
    }

    private void SetStats()
    {
        int weaponAttack = Hunter.Weapon != null ? Hunter.Weapon.Attack : 0;
        HBoxContainer attackNode = GetStatNode(StatType.Attack, $"{weaponAttack}");
        _statContainer.AddChild(attackNode);

        int armorDefense = GetArmorDefense();
        HBoxContainer defenseNode = GetStatNode(StatType.Defense, $"{armorDefense}");
        _statContainer.AddChild(defenseNode);

        int weaponAffinity = Hunter.Weapon != null ? Hunter.Weapon.Affinity : 0;
        HBoxContainer affinityNode = GetStatNode(StatType.Affinity, $"{weaponAffinity}%");
        _statContainer.AddChild(affinityNode);

        if (Hunter.Weapon.Special != SpecialType.None && Hunter.Weapon.SpecialAttack != 0)
        {
            HBoxContainer specialAttackNode = GetStatNode(Hunter.Weapon.Special, $"{Hunter.Weapon.SpecialAttack}");
            _statContainer.AddChild(specialAttackNode);
        }
    }

    private void ClearStats()
    {
        foreach (Node child in _statContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    // Go through all armor pieces and get their defense value
    private int GetArmorDefense()
    {
        int hunterDefense = 0;

        if (Hunter.Head.Name != "") hunterDefense += Hunter.Head.Defense;
        if (Hunter.Chest.Name != "") hunterDefense += Hunter.Chest.Defense;
        if (Hunter.Arm.Name != "") hunterDefense += Hunter.Arm.Defense;
        if (Hunter.Waist.Name != "") hunterDefense += Hunter.Waist.Defense;
        if (Hunter.Leg.Name != "") hunterDefense += Hunter.Leg.Defense;

        return hunterDefense;
    }

    private void EquipmentChanged(Equipment equipment)
    {
        ClearStats();
        SetStats();
    }

    private HBoxContainer GetStatNode<T>(T enumType, string statValueString)
    {
        HBoxContainer statNode = new HBoxContainer()
        {
            Alignment = BoxContainer.AlignmentMode.Center,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        statNode.AddThemeConstantOverride("separation", 0);

        Texture2D texture = new Texture2D();
        if (enumType is StatType statType)
        {
            texture = MonsterHunterIdle.GetStatTypeIcon(statType);
        }
        else if (enumType is SpecialType specialType)
        {
            texture = MonsterHunterIdle.GetSpecialTypeIcon(specialType);
        }

        TextureRect statIcon = new TextureRect()
        {
            Texture = texture,
            CustomMinimumSize = new Vector2(40, 40),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };

        Label statLabel = new Label()
        {
            Text = statValueString
        };
        statLabel.AddThemeFontSizeOverride("font_size", 16);

        statNode.AddChild(statIcon);
        statNode.AddChild(statLabel);

        return statNode;
    }
}
