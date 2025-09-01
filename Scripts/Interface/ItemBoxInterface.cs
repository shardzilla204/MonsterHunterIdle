using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MonsterHunterIdle;

public partial class ItemBoxInterface : Container
{
	[Export]
	private Container _parentContainer;

	[Export]
	private ItemBoxSearch _itemBoxSearch;

	[Export]
	private CustomButton _sellModeButton;

	private SellMaterialLogContainer _sellMaterialLogContainer;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.LocaleMaterialAdded -= OnLocaleMaterialAdded;
		MonsterHunterIdle.Signals.MonsterMaterialAdded -= OnMonsterMaterialAdded;
		MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.LocaleMaterialAdded += OnLocaleMaterialAdded;
		MonsterHunterIdle.Signals.MonsterMaterialAdded += OnMonsterMaterialAdded;
		MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
	}

	public override void _Ready()
	{
		_itemBoxSearch.FilterChanged += FilterMaterialLogs;
		_sellModeButton.Toggled += SellMode;

		SellMode(false);
	}

	// * START - Signal Methods
	private void OnLocaleMaterialAdded(LocaleMaterial localeMaterial)
	{
		UpdateMaterialLogs();
	}

	private void OnMonsterMaterialAdded(MonsterMaterial monsterMaterial)
	{
		UpdateMaterialLogs();
	}

	private void OnInterfaceChanged(InterfaceType interfaceType)
	{
		QueueFree();
	}
	// * END - Signal Methods

	private void SellMode(bool isToggled)
	{
		Color green = Color.FromHtml(PrintRich.GetColorHex(TextColor.Green));
		Color red = Color.FromHtml(PrintRich.GetColorHex(TextColor.Red));
		Color buttonColor = isToggled ? green : red;
		_sellModeButton.SetColor(buttonColor);

		UpdateMaterialLogs();
		_itemBoxSearch.Text = "";
	}

	private void UpdateMaterialLogs()
	{
		GC.Dictionary<string, int> materialsToSell = new GC.Dictionary<string, int>();
		if (_sellModeButton.ButtonPressed && IsInstanceValid(_sellMaterialLogContainer))
		{
			materialsToSell = _sellMaterialLogContainer.MaterialsToSell;
		}

		ClearDisplay();

		List<Material> distinctMaterials = ItemBox.Materials.Distinct().ToList();

		if (!_sellModeButton.ButtonPressed)
		{
			AddMaterialLogs(distinctMaterials);
		}
		else
		{
			AddSellMaterialContainer(distinctMaterials, materialsToSell);
		}
	}

	private void AddMaterialLogs(List<Material> materials)
	{
		Container materialLogContainerNode = GetMaterialLogContainerNode();
		FlowContainer flowContainer = materialLogContainerNode.GetChild<FlowContainer>(0);
		foreach (Material material in materials)
		{
			MaterialLog materialLog = MonsterHunterIdle.PackedScenes.GetMaterialLog(material);
			flowContainer.AddChild(materialLog);
		}
		_parentContainer.AddChild(materialLogContainerNode);
	}

	private void AddSellMaterialContainer(List<Material> materials)
	{
		GC.Array<Material> materialsArray = [.. materials];
		_sellMaterialLogContainer = MonsterHunterIdle.PackedScenes.GetSellMaterialLogContainer();
		_sellMaterialLogContainer.SellButtonPressed += SellMaterials;
		_sellMaterialLogContainer.AddSellMaterialLogs(materialsArray);
		_parentContainer.AddChild(_sellMaterialLogContainer);
	}

	// Keep the amount to sell when changing the filter
	private void AddSellMaterialContainer(List<Material> materials, GC.Dictionary<string, int> materialsToSell)
	{
		AddSellMaterialContainer(materials);

		foreach (string materialName in materialsToSell.Keys)
		{
			int amount = materialsToSell[materialName];
			SellMaterialLog sellMaterialLog = _sellMaterialLogContainer.FindSellMaterialLog(materialName);
			sellMaterialLog.SetAmount(amount);
		}
	}

	private void FilterMaterialLogs(GC.Array<Material> filteredMaterials)
	{
		GC.Dictionary<string, int> materialsToSell = new GC.Dictionary<string, int>();
		if (_sellModeButton.ButtonPressed)
		{
			materialsToSell = _sellMaterialLogContainer.MaterialsToSell;
		}

		ClearDisplay();

		List<Material> distinctMaterials = filteredMaterials.Count == 0 ? ItemBox.Materials.Distinct().ToList() : filteredMaterials.ToList();
		if (!_sellModeButton.ButtonPressed)
		{
			AddMaterialLogs(distinctMaterials);
		}
		else
		{
			AddSellMaterialContainer(distinctMaterials, materialsToSell);
		}
	}

	private void ClearDisplay()
	{
		foreach (Container childContainer in _parentContainer.GetChildren())
		{
			childContainer.QueueFree();
		}
		
		_sellMaterialLogContainer = null;
	}

	private ScrollContainer GetMaterialLogContainerNode()
	{
		ScrollContainer materialContainerNode = new ScrollContainer()
		{
			FollowFocus = true,
			HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
			VerticalScrollMode = ScrollContainer.ScrollMode.ShowNever,
			SizeFlagsVertical = SizeFlags.ExpandFill 
		};

		int separationValue = 11;
		FlowContainer flowContainer = new FlowContainer()
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill
		};
		flowContainer.AddThemeConstantOverride("h_separation", separationValue);
		flowContainer.AddThemeConstantOverride("v_separation", separationValue);

		materialContainerNode.AddChild(flowContainer);
		return materialContainerNode;
	}

	private void SellMaterials(GC.Dictionary<string, int> materialsToSell)
	{
		foreach (string materialName in materialsToSell.Keys)
		{
			Material material = MonsterHunterIdle.FindMaterial(materialName);
			int amount = materialsToSell[materialName];
			ItemBox.SubtractMaterial(material, amount);

			int zenny = ItemBox.GetSellValue(material) * amount;
			HunterManager.AddZenny(zenny);

			string soldMessage = $"Sold {amount} {materialName} For {zenny} Zenny";
			PrintRich.PrintLine(TextColor.Yellow, soldMessage);
		}
	}
}
