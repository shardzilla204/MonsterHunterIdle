using System.Collections.Generic;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class SellMaterialLogContainer : VBoxContainer
{
    [Signal]
    public delegate void SellButtonPressedEventHandler(GC.Dictionary<string, int> materialsToSell);

    [Export]
    private VBoxContainer _sellMaterialLogContainer;

    [Export]
    private CustomButton _sellButton;

    [Export]
    private Label _zennyAmount;

    private List<SellMaterialLog> _sellMaterialLogs = new List<SellMaterialLog>();

    public GC.Dictionary<string, int> MaterialsToSell = new GC.Dictionary<string, int>();

    public override void _Ready()
    {
        _sellButton.Pressed += OnSellButtonPressed;
    }

    public void AddSellMaterialLogs(GC.Array<Material> materials)
    {
        foreach (Material material in materials)
        {
            SellMaterialLog sellMaterialLog = MonsterHunterIdle.PackedScenes.GetSellMaterialLog(material);
            sellMaterialLog.AmountChanged += AddMaterialToSell;
            _sellMaterialLogs.Add(sellMaterialLog);
            AddChild(sellMaterialLog);
        }
    }

    public void AddChild(SellMaterialLog sellMaterialLog)
    {
        _sellMaterialLogContainer.AddChild(sellMaterialLog);
    }

    private void AddMaterialToSell(Material material, int amount)
    {
        bool hasMaterial = MaterialsToSell.ContainsKey(material.Name);
        if (hasMaterial)
        {
            MaterialsToSell[material.Name] = amount;
        }
        else
        {
            MaterialsToSell.Add(material.Name, amount);
        }

        SetZennySellTotal();
    }

    private void SetZennySellTotal()
    {
        int totalZenny = 0;
        foreach (string materialName in MaterialsToSell.Keys)
        {
            Material material = MonsterHunterIdle.FindMaterial(materialName);
            int amount = MaterialsToSell[materialName];

            int zenny = MonsterHunterIdle.ItemBox.GetSellValue(material) * amount;
            totalZenny += zenny;
        }

        _zennyAmount.Text = $"{totalZenny}";
    }

    private void OnSellButtonPressed()
    {
        if (MaterialsToSell.Count <= 0) return;

        EmitSignal(SignalName.SellButtonPressed, MaterialsToSell);
        foreach (SellMaterialLog sellMaterialLog in _sellMaterialLogContainer.GetChildren())
        {
            sellMaterialLog.Reset();
        }
    }

    public SellMaterialLog FindSellMaterialLog(string targetMaterialName)
    {
        return _sellMaterialLogs.Find(log => log.Material.Name == targetMaterialName);
    }
}
