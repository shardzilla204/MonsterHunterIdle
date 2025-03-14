using Godot;

namespace MonsterHunterIdle;

public partial class Signals : Node
{
	[Signal]
	public delegate void PalicoRecruitedEventHandler();

	[Signal]
	public delegate void PalicoGatheredEventHandler(Palico palico);

	[Signal]
	public delegate void PalicoHuntedEventHandler(Palico palico);

	[Signal]
	public delegate void OpenedPalicoLoadoutEventHandler(Palico palico);

	[Signal]
	public delegate void ClosedPalicoLoadoutEventHandler(Palico palico);

	[Signal]
	public delegate void MonsterEncounteredEventHandler(Monster monster);

	[Signal]
	public delegate void MonsterDamagedEventHandler(int damage);

	[Signal]
	public delegate void MonsterSlayedEventHandler(Monster monster);

	[Signal]
	public delegate void MonsterLeftEventHandler();

	[Signal]
	public delegate void PlayerLeveledUpEventHandler();

	[Signal]
	public delegate void MaterialAddedEventHandler(Material material, int quantity);

	[Signal]
	public delegate void MaterialUsedEventHandler(Material material, int quantity);

	[Signal]
	public delegate void ChangedPalicoModeEventHandler();

	[Signal]
	public delegate void ChangedLocaleEventHandler();

}
