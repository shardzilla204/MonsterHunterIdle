using Godot;

namespace MonsterHunterIdle;

public partial class CraftButton : CustomButton
{
	[Export]
	private Label _equipmentName;

	[Export]
	private TextureRect _equipmentIcon;

	[Export]
	private TextureRect _hasCreatedTexture;

	public Equipment Equipment;

	public override void _Ready()
	{
		base._Ready();
		Pressed += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.CraftButtonPressed, Equipment);
	}

	public void SetEquipment(Equipment equipment)
	{
		if (equipment == null) return;

		bool hasCrafted = MonsterHunterIdle.EquipmentManager.HasCrafted(equipment);
		_hasCreatedTexture.SelfModulate = hasCrafted ? Colors.SeaGreen : Colors.White;

		equipment = hasCrafted ? MonsterHunterIdle.EquipmentManager.GetHunterEquipment(equipment) : equipment;
		string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";
		_equipmentName.Text = $"{equipment.Name}{subGrade}";

		Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
		_equipmentIcon.Texture = equipmentIcon;

		Equipment = equipment;
	}
}
