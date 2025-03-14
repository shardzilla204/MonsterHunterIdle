using Godot;

namespace MonsterHunterIdle;

public partial class MaterialLog : NinePatchRect
{
	[Export]
	private Label _materialName;

	[Export]
	private TextureRect _materialIcon;

	[Export]
	private Label _materialQuantity;

	public new MaterialData Material;

    public override void _Ready()
    {
      _materialName.Text = Material.Name;
		_materialIcon.Texture = Material.Icon;
		_materialQuantity.Text = Material.Quantity.ToString();
    }
}
