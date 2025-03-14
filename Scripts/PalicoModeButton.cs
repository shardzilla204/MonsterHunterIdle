using Godot;

namespace MonsterHunterIdle;

public partial class PalicoModeButton : CustomButton
{
	[Export]
	private TextureRect _modeIcon;

	private Palico _palico;

	public override void _Ready()
	{
		base._Ready();
		Pressed += ChangeModes;
	}

	private void ChangeModes()
	{
		_palico.Mode = _palico.Mode switch
		{
			PalicoMode.Gather => PalicoMode.Hunt,
			PalicoMode.Hunt => PalicoMode.Gather,
			_ => PalicoMode.Gather
		};
		SetPalicoMode(_palico);
		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.ChangedPalicoMode);
	}

	public void SetPalicoMode(Palico palico)
	{
		_palico = palico;
		_modeIcon.Texture = palico.Mode switch
		{
			PalicoMode.Gather => MonsterHunterIdle.BiomeManager.Biome.GatherIcon,
			PalicoMode.Hunt => MonsterHunterIdle.GetIcon("Hunt"),
			_ => MonsterHunterIdle.BiomeManager.Biome.GatherIcon
		};
	}
}
