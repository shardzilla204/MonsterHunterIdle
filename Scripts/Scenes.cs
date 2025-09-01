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
}