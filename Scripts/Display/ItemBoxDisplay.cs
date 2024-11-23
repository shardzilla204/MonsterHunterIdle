using Godot;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class ItemBoxDisplay : NinePatchRect
{
	[Export]
	private PackedScene _materialDisplayScene;

	[Export]
	private Container _materialContainer;

	public override void _Ready()
	{
		CreateMaterialDisplays();
	}

	private void CreateMaterialDisplays()
	{
		IEnumerable<dynamic> uniqueMaterials = GetUniqueMaterials();

		foreach(dynamic uniqueItem in uniqueMaterials)
		{
			MaterialDisplay materialDisplay = _materialDisplayScene.Instantiate<MaterialDisplay>();
			materialDisplay.Material = uniqueItem;
			_materialContainer.AddChild(materialDisplay);
		}
	}	

	private IEnumerable<dynamic> GetUniqueMaterials()
	{
		ItemBoxData itemBox = GameManager.Instance.ItemBox;
		return itemBox.Materials.Distinct();
	}
}
