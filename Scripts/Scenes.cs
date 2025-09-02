using Godot;

namespace MonsterHunterIdle;

public partial class Scenes : Control
{
    public static HBoxContainer GetInfoNode(Texture2D texture, string text)
    {
        HBoxContainer infoNode = new HBoxContainer();

        int size = 40;
        TextureRect iconTextureRect = new TextureRect()
        {
            Texture = texture,
            CustomMinimumSize = new Vector2(size, size),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };
        infoNode.AddChild(iconTextureRect);

        Label nameLabel = new Label()
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Right,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        int fontSize = 20;
        nameLabel.AddThemeFontSizeOverride("font_size", fontSize);
        infoNode.AddChild(nameLabel);

        return infoNode;
    }

    public static Label GetDamageLabel(Control control, int damage, TextColor color)
    {
        string redColorHex = PrintRich.GetColorHex(color);
        Vector2 size = new Vector2(40, 100);
        Label damageLabel = new Label()
        {
            MouseFilter = MouseFilterEnum.Ignore,
            Text = $"+ {damage}",
            Size = size,
            Position = control.GetLocalMousePosition() - (size / 2),
            SelfModulate = Color.FromHtml(redColorHex),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        int fontSize = 30;
        damageLabel.AddThemeFontSizeOverride("font_size", fontSize);

        return damageLabel;
    }

    public static HBoxContainer GetSpecialAttackContainer(Control control, int specialAttack)
    {
        HBoxContainer specialAttackContainer = new HBoxContainer()
        {
            MouseFilter = MouseFilterEnum.Ignore
        };
        int separation = 0;
        specialAttackContainer.AddThemeConstantOverride("separation", separation);

        int textureSize = 50;
        Texture2D specialTypeIcon = MonsterHunterIdle.GetSpecialTypeIcon(Hunter.Weapon.Special);
        TextureRect specialTexture = new TextureRect()
        {
            MouseFilter = MouseFilterEnum.Ignore,
            Texture = specialTypeIcon,
            CustomMinimumSize = new Vector2(textureSize, textureSize),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };

        Label damageLabel = GetDamageLabel(control, specialAttack, TextColor.Orange);

        specialAttackContainer.AddChild(damageLabel);
        specialAttackContainer.AddChild(specialTexture);

        specialAttackContainer.Position = control.GetLocalMousePosition() - (specialAttackContainer.Size / 2);

        return specialAttackContainer;
    }

    public static CustomButton GetForgeButton()
    {
        int size = 50;
        CustomButton forgeButton = new CustomButton()
        {
            CustomMinimumSize = new Vector2(size, size),
            Flat = true
        };

        string textureFilePath = "res://Assets/Images/RoundedSquare.png";
        int textureMargin = 10;
        NinePatchRect ninePatchRect = new NinePatchRect()
        {
            Texture = MonsterHunterIdle.GetTexture(textureFilePath),
            PatchMarginLeft = textureMargin,
            PatchMarginTop = textureMargin,
            PatchMarginRight = textureMargin,
            PatchMarginBottom = textureMargin,
            SelfModulate = Color.FromHtml("#404040")
        };
        ninePatchRect.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        int fontSize = 34;
        Label label = new Label()
        {
            Text = "+",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        label.AddThemeConstantOverride("font_size", fontSize);
        label.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        ninePatchRect.AddChild(label);
        forgeButton.AddChild(ninePatchRect);

        return forgeButton;
    }
    
    public static HBoxContainer GetLoadoutStat<T>(T enumType, string statValueString)
    {
        int separation = 0;
        HBoxContainer loadoutStat = new HBoxContainer()
        {
            Alignment = BoxContainer.AlignmentMode.Center,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        loadoutStat.AddThemeConstantOverride("separation", separation);

        Texture2D texture = new Texture2D();
        if (enumType is StatType statType)
        {
            texture = MonsterHunterIdle.GetStatTypeIcon(statType);
        }
        else if (enumType is SpecialType specialType)
        {
            texture = MonsterHunterIdle.GetSpecialTypeIcon(specialType);
        }

        int iconSize = 40;
        TextureRect statIcon = new TextureRect()
        {
            Texture = texture,
            CustomMinimumSize = new Vector2(iconSize, iconSize),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };

        int fontSize = 16;
        Label statLabel = new Label()
        {
            Text = statValueString
        };
        statLabel.AddThemeFontSizeOverride("font_size", fontSize);

        loadoutStat.AddChild(statIcon);
        loadoutStat.AddChild(statLabel);

        return loadoutStat;
    }
}