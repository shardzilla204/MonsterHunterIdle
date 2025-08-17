using Godot;

namespace MonsterHunterIdle;

public partial class CraftingMaterialLog : NinePatchRect
{
    [Export]
    private TextureRect _materialIcon;

    [Export]
    private Label _materialName;

    [Export]
    private Label _materialAmount;

    public new Material Material;
    public int Amount;

    public void SetMaterial(Material material, int amount)
    {
        Material = material;
        Amount = amount;

        _materialIcon.Texture = material == null ? null : MonsterHunterIdle.GetMaterialIcon(material);
        _materialName.Text = material == null ? "" : material.Name;

        int itemBoxAmount = MonsterHunterIdle.ItemBox.FindAllMaterial(material.Name).Count;
        _materialAmount.Text = material == null ? "" : $"{itemBoxAmount}  / {amount}";
    }
}
