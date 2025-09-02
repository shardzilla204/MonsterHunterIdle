using System;
using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class SellMaterialLog : MaterialLog
{
    [Signal]
    public delegate void AmountChangedEventHandler(Material material, int amount);

    [Export]
    protected new LineEdit _materialAmount;

    [Export]
    private Button _addButton;

    [Export]
    private Button _subtractButton;

    public int Amount;

    public override void _Ready()
    {
        _materialAmount.TextSubmitted += OnTextSubmitted;
        _addButton.Pressed += OnAddButtonPressed;
        _subtractButton.Pressed += OnSubtractButtonPressed;
    }
    // * START - Signal Methods
    private void OnTextSubmitted(string newText)
    {
        List<Material> materials = ItemBox.Materials.FindAll(material => material == Material);
        int maxMaterialCount = materials.Count;
        int targetMaterialCount = int.Parse(newText);

        Amount = Math.Min(targetMaterialCount, maxMaterialCount);

        _materialAmount.Text = $"{Amount}";
        EmitSignal(SignalName.AmountChanged, Material, Amount);
    }

    private void OnAddButtonPressed()
    {
        List<Material> materials = ItemBox.Materials.FindAll(material => material == Material);
        int maxMaterialCount = materials.Count;

        Amount = Math.Min(maxMaterialCount, ++Amount);
        _materialAmount.Text = $"{Amount}";
        EmitSignal(SignalName.AmountChanged, Material, Amount);
    }

    private void OnSubtractButtonPressed() 
    {
        Amount = Math.Max(0, --Amount);
        _materialAmount.Text = $"{Amount}";
        EmitSignal(SignalName.AmountChanged, Material, Amount);
    }

    // * END - Signal Methods

    // Set the text to zero 
    public override void SetMaterialAmount(Material targetMaterial)
    {
        _materialAmount.Text = "0";
    }

    public void SetAmount(int amount)
    {
        Amount = amount;
        _materialAmount.Text = $"{Amount}";
        EmitSignal(SignalName.AmountChanged, Material, Amount);
    }
}
