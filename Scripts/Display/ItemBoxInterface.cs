using Godot;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class ItemBoxInterface : NinePatchRect
{
	[Export]
	private PackedScene _materialDisplayScene;

	[Export]
	private Container _materialContainer;

	[Export]
	private ItemBoxSearch _searchBar;

	public override void _Ready()
	{
		ClearDisplay();
		CreateMaterialLogs();
		// CollectionLogManager.Instance.Updated += Update;
		// TreeExited += () => CollectionLogManager.Instance.Updated -= Update;
		_searchBar.ItemBoxDisplay = this;
	}

	private void Update()
	{
		if (!IsInstanceValid(this)) return;
		
		ClearDisplay();
		CreateMaterialLogs();
	}

	private void CreateMaterialLogs()
	{
		IEnumerable<dynamic> uniqueMaterials = GetUniqueMaterials();

		foreach (dynamic uniqueMaterial in uniqueMaterials)
		{
			MaterialLog materialLog = _materialDisplayScene.Instantiate<MaterialLog>();
			materialLog.Material = uniqueMaterial;
			_materialContainer.AddChild(materialLog);
		}
	}	

	public void FilterMaterialLogs(List<dynamic> materialsFromFilter)
	{
		ClearDisplay();
		if (materialsFromFilter is null)
		{
			CreateMaterialLogs();
			return;
		}
		else
		{
			foreach (dynamic material in materialsFromFilter)
			{
				MaterialLog materialLog = _materialDisplayScene.Instantiate<MaterialLog>();
				materialLog.Material = material;
				_materialContainer.AddChild(materialLog);
			}
		}
	}

	private IEnumerable<dynamic> GetUniqueMaterials()
	{
		ItemBoxData itemBox = GameManager.Instance.ItemBox;
		return itemBox.Materials.Distinct();
	}

	private void ClearDisplay()
	{
		foreach (Node materialLog in _materialContainer.GetChildren())
		{
			_materialContainer.RemoveChild(materialLog);
			materialLog.QueueFree();
		}
	}
}
