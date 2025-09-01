using Godot;
using Godot.Collections;

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

	// Game Signals
	[Signal]
	public delegate void GameSavedEventHandler();

	[Signal]
	public delegate void GameLoadedEventHandler();

	[Signal]
	public delegate void GameDeletedEventHandler();

	[Signal]
	public delegate void GameQuitEventHandler();

	// Miscellaneous Signals
	[Signal]
	public delegate void CollectionLogTimedOutEventHandler(CollectionLog collectionLog);

	[Signal]
	public delegate void InterfaceChangedEventHandler(InterfaceType interfaceType);

	[Signal]
	public delegate void CraftButtonPressedEventHandler(Equipment equipment);

	[Signal]
	public delegate void PalicoCraftButtonPressedEventHandler(PalicoEquipment equipment);

	[Signal]
	public delegate void PalicoCraftOptionButtonPressedEventHandler(PalicoEquipment equipment, bool isCrafting, int index);

	[Signal]
	public delegate void EquipmentUpgradedEventHandler(Equipment equipment);

	[Signal]
	public delegate void PalicoEquipmentUpgradedEventHandler(PalicoEquipment equipment, int index);

	[Signal]
	public delegate void EquipmentAddedEventHandler(Equipment equipment);

	[Signal]
	public delegate void PalicoEquipmentAddedEventHandler(PalicoEquipment equipment, int index);

	[Signal]
	public delegate void EquipmentChangedEventHandler(Equipment equipment);

	[Signal]
	public delegate void PalicoEquipmentChangedEventHandler(PalicoEquipment equipment);

	[Signal]
	public delegate void ChangeEquipmentButtonPressedEventHandler(Equipment equipment);

	[Signal]
	public delegate void ChangePalicoEquipmentButtonPressedEventHandler(PalicoEquipmentType equipmentType);

	[Signal]
	public delegate void PopupEventHandler(Control node);

	[Signal]
	public delegate void FilterButtonToggledEventHandler(bool isToggled);

	[Signal]
	public delegate void FiltersChangedEventHandler(Dictionary<string, bool> filters);
}
