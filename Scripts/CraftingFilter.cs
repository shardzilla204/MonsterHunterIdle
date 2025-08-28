using System;
using Godot;

namespace MonsterHunterIdle;

public partial class CraftingFilter : HBoxContainer
{
    [Signal]
    public delegate void FilterToggledEventHandler(bool isToggled);

    [Signal]
    public delegate void ButtonPressedEventHandler(bool isToggled);

    [Export]
    private CheckBox _checkBox;

    [Export]
    private TextureRect _filterIcon;

    public Enum Category;

    public TextureRect FilterIcon => _filterIcon;

    public override void _Ready()
    {
        _checkBox.Toggled += OnFilterToggled;
    }

    public void SetTexture(string fileName)
    {
        string folderPath = "res://Assets/Images/Filter/";
        string filePath = $"{folderPath}/{fileName}Filter.png";

        Texture2D filterIcon = MonsterHunterIdle.GetTexture(filePath);
        _filterIcon.Texture = filterIcon;
    }

    private void OnFilterToggled(bool isToggled)
    {
        EmitSignal(SignalName.ButtonPressed, isToggled); // For unchecking the other filters
        EmitSignal(SignalName.FilterToggled, isToggled);
    }

    public void ToggleFilter(bool isToggled)
    {
        _checkBox.ButtonPressed = isToggled;
    }
}
