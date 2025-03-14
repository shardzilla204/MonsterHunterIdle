using Godot;

public partial class EquipmentDisplay : CustomButton
{
	[Export]
	public TextureRect EquipmentIcon;

	[Export]
	private Label _equipmentName;

	[Export]
	private TextureRect _hasCreated;

	public dynamic EquipmentData;

    public override void _Ready()
    {
		base._Ready();
		if (EquipmentData is null) return;

		_equipmentName.Text = EquipmentData.Name;
		HasCreated();
		Pressed += () => 
		{
			if (EquipmentData.HasCreated)
			{
				ShowUpgradeDisplay();
			}
			else
			{
				ShowCraftingDisplay();
			}
		};
    }

	private void HasCreated()
	{
		_hasCreated.SelfModulate = EquipmentData.HasCreated ? Colors.Green : Colors.Red;
	}

	private void ShowCraftingDisplay()
	{

	}

	private void ShowUpgradeDisplay()
	{

	}
}
