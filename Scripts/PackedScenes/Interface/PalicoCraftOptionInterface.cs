using Godot;
using System.Collections.Generic;
using System.Reflection;

namespace MonsterHunterIdle;

public partial class PalicoCraftOptionInterface : NinePatchRect
{
    [Export]
    private Container _palicoCraftButtonOptionContainer;

    public PalicoEquipment Equipment;
    public int Index;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.PalicoEquipmentAdded -= OnPalicoEquipmentAdded;
        MonsterHunterIdle.Signals.PalicoEquipmentUpgraded -= OnPalicoEquipmentUpgraded;
        MonsterHunterIdle.Signals.EquipmentAdded -= OnEquipmentEvent;
        MonsterHunterIdle.Signals.EquipmentUpgraded -= OnEquipmentEvent;
        MonsterHunterIdle.Signals.CraftButtonPressed -= OnEquipmentEvent;
        MonsterHunterIdle.Signals.PalicoCraftButtonPressed -= OnEquipmentEvent;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.PalicoEquipmentAdded += OnPalicoEquipmentAdded;
        MonsterHunterIdle.Signals.PalicoEquipmentUpgraded += OnPalicoEquipmentUpgraded;
        MonsterHunterIdle.Signals.EquipmentUpgraded += OnEquipmentEvent;
        MonsterHunterIdle.Signals.EquipmentAdded += OnEquipmentEvent;
        MonsterHunterIdle.Signals.CraftButtonPressed += OnEquipmentEvent;
        MonsterHunterIdle.Signals.PalicoCraftButtonPressed += OnEquipmentEvent;
    }

    public override void _Ready()
    {
        SetPalicoCraftOptionButtons();
    }

    // * START - Signal Methods
    private void OnPalicoEquipmentAdded(PalicoEquipment equipment, int index)
    {
        QueueFree();
    }

    private void OnPalicoEquipmentUpgraded(PalicoEquipment equipment, int index)
    {
        QueueFree();
    }

    private void OnEquipmentEvent(Equipment equipment)
    {
        QueueFree();
    }
    // * END - Signal Methods

    private void SetPalicoCraftOptionButtons()
    {
        if (PalicoManager.Palicos.Count == 0)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = "There Are No Palicos";
            PrintRich.PrintError(className, message);

            return;
        }

        // Show all the equipment of that type, that's already been created
        List<PalicoEquipment> equipmentPieces = PalicoEquipmentManager.FindCraftedEquipment(Equipment);
        foreach (PalicoEquipment equipmentPiece in equipmentPieces)
        {
            PalicoCraftOptionButton palicoCraftOptionButton = MonsterHunterIdle.PackedScenes.GetPalicoCraftOptionButton(equipmentPiece);
            palicoCraftOptionButton.Pressed += () => OnButtonPressed(equipmentPiece, false);
            _palicoCraftButtonOptionContainer.AddChild(palicoCraftOptionButton);
        }

        PalicoEquipment equipment = Equipment;
        if (Equipment is PalicoWeapon targetWeapon)
        {
            PalicoWeapon weaponCopy = PalicoEquipmentManager.GetWeapon(targetWeapon.Tree);
            equipment = weaponCopy;
        }
        else if (Equipment is PalicoArmor targetArmor)
        {
            PalicoArmor armorCopy = PalicoEquipmentManager.GetArmor(targetArmor.Type, targetArmor.Set);
            equipment = armorCopy;
        }

        bool canCraft = PalicoEquipmentManager.CanCraft(equipment);
        if (canCraft)
        {
            CustomButton forgeButton = Scenes.GetForgeButton();
            forgeButton.Pressed += () =>
            {
                PrintRich.PrintEquipmentInfo(TextColor.LightBlue, equipment);
                OnButtonPressed(equipment, true);
            };
            _palicoCraftButtonOptionContainer.AddChild(forgeButton);
        }
    }

    private void OnButtonPressed(PalicoEquipment equipment, bool isCrafting)
    {
        int index = -1;
        if (equipment is PalicoWeapon targetWeapon)
        {
            List<PalicoWeapon> weapons = PalicoEquipmentManager.FindCraftedWeapons(targetWeapon);
            index = weapons.IndexOf(targetWeapon);
        }
        else if (equipment is PalicoArmor targetArmor)
        {
            List<PalicoArmor> armor = PalicoEquipmentManager.FindCraftedArmor(targetArmor);
            index = armor.IndexOf(targetArmor);
        }
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoCraftOptionButtonPressed, equipment, isCrafting, index);
    }
}
