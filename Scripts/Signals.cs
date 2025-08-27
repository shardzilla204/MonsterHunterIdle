using Godot;

namespace MonsterHunterIdle;

public partial class Signals : Node
{
	// Palico Signals
	[Signal]
	public delegate void PalicoRecruitedEventHandler();

	[Signal]
	public delegate void PalicoGatheredEventHandler(Palico palico);

	[Signal]
	public delegate void PalicoHuntedEventHandler(Palico palico);

	[Signal]
	public delegate void PalicoModeChangedEventHandler();

	// Monster Signals
	[Signal]
	public delegate void MonsterEncounteredEventHandler(Monster monster);

	[Signal]
	public delegate void MonsterDamagedEventHandler(int damage);

	[Signal]
	public delegate void MonsterSlayedEventHandler(Monster monster);

	[Signal]
	public delegate void MonsterLeftEventHandler();

	[Signal]
	public delegate void MonsterEncounterFinishedEventHandler();

	[Signal]
	public delegate void MonsterMaterialAddedEventHandler(MonsterMaterial monsterMaterial);

	[Signal]
	public delegate void MonsterMaterialUsedEventHandler(MonsterMaterial monsterMaterial);

	// Hunter Signals
	[Signal]
	public delegate void HunterLeveledUpEventHandler();

	[Signal]
	public delegate void HunterAttackedEventHandler();

	[Signal]
	public delegate void HunterPointsChangedEventHandler();

	// Locale Signals
	[Signal]
	public delegate void LocaleChangedEventHandler();

	[Signal]
	public delegate void LocaleMaterialAddedEventHandler(LocaleMaterial localeMaterial);

	[Signal]
	public delegate void LocaleMaterialUsedEventHandler(LocaleMaterial localeMaterial);

	// Miscellaneous Signals
	[Signal]
	public delegate void CollectionLogTimedOutEventHandler(CollectionLog collectionLog);

	[Signal]
	public delegate void InterfaceChangedEventHandler(InterfaceType interfaceType);

	[Signal]
	public delegate void CraftButtonPressedEventHandler(Equipment equipment);

	[Signal]
	public delegate void EquipmentUpgradedEventHandler(Equipment equipment);

	[Signal]
	public delegate void WeaponAddedEventHandler(Weapon weapon);

	[Signal]
	public delegate void ArmorAddedEventHandler(Armor armor);

	[Signal]
	public delegate void EquipmentChangedEventHandler(Equipment equipment);

	[Signal]
	public delegate void ChangeEquipmentButtonPressedEventHandler(Equipment equipment);

	[Signal]
	public delegate void PopupEventHandler(Control node);

	[Signal]
	public delegate void GameSavedEventHandler();

	[Signal]
	public delegate void GameLoadedEventHandler();

	[Signal]
	public delegate void GameDeletedEventHandler();

	[Signal]
	public delegate void GameQuitEventHandler();
}
