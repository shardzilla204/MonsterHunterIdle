namespace MonsterHunterIdle;

public enum PalicoEquipmentType
{
    None = -1,
    Weapon,
    Head,
    Chest
}

public abstract partial class PalicoEquipment : Equipment
{
    public new PalicoEquipmentType Type = PalicoEquipmentType.None;
}