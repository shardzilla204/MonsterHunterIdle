using Godot;

namespace MonsterHunterIdle;

public partial class PalicoLoadout : NinePatchRect
{
	[Export]
	private Label _palicoName;

	[Export]
	private PalicoEquipmentDetails _weaponsDetails;

	[Export]
	private PalicoEquipmentDetails _helmetDetails;

	[Export]
	private PalicoEquipmentDetails _armorDetails;

	public PalicoData Palico;

	public override void _Ready()
	{
		_palicoName.Text = $"{Palico.Name}'s Loadout";
		_weaponsDetails.GetStats(Palico);
		_helmetDetails.GetStats(Palico);
		_armorDetails.GetStats(Palico);
	}

	public override void _Process(double delta)
	{
		
	}
}
