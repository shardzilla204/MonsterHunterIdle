using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class MaterialLog : Control
{
	[Export]
	private Label _materialName;

	[Export]
	private TextureRect _materialIcon;

	[Export]
	protected Label _materialAmount;

	public new Material Material;

	public void SetMaterial(Material targetMaterial, int amount = 0)
	{
		Material = targetMaterial;

		_materialName.Text = targetMaterial.Name;
		_materialIcon.Texture = MonsterHunterIdle.GetMaterialIcon(targetMaterial);

		if (amount == 0)
		{
			SetMaterialAmount(targetMaterial);
		}
		else
		{
			_materialAmount.Text = $"{amount}";
		}
	}

	public virtual void SetMaterialAmount(Material targetMaterial)
	{
		List<Material> targetMaterials = ItemBox.Materials.FindAll(material => material == targetMaterial);
		_materialAmount.Text = $"{targetMaterials.Count}";
	}
}
