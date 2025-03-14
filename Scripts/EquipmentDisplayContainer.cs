using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class EquipmentDisplayContainer : NinePatchRect
{
	private enum EntityType
	{
		Player,
		Palico
	}

	private enum EquipmentType
	{
		Weapon,
		Armor
	}

	[Export]
	private PackedScene _equipmentDisplayScene;

	[Export]
	private Container _container;

	[Export]
	private Texture2D _weaponIcon;

	private EntityType _entityType = EntityType.Player;
	private EquipmentType _equipmentType = EquipmentType.Weapon;

    public override void _Ready()
    {
		ClearEntityEquipment();
        ShowEntityEquipment();
    }

    public void ButtonChanged(int typeValue)
	{
		ButtonGroup.Type buttonGroupType = (ButtonGroup.Type) typeValue;

		if (buttonGroupType is ButtonGroup.Type.Entity)
		{
			EntityGroupChanged();
		}
		else
		{
			EquipmentGroupChanged();
		}
		ClearEntityEquipment();
		ShowEntityEquipment();
	}

	private void EntityGroupChanged()
	{
		if (_entityType is EntityType.Player)
		{
			_entityType = EntityType.Palico;
		} 
		else
		{
			_entityType = EntityType.Player;
		}
	}

	private void EquipmentGroupChanged()
	{
		if (_equipmentType is EquipmentType.Weapon)
		{
			_equipmentType = EquipmentType.Armor;
		} 
		else
		{
			_equipmentType = EquipmentType.Weapon;
		}
	}

	private void ShowEntityEquipment()
	{
		if (_entityType is EntityType.Player)
		{
			PlayerDataHolder player = DataManager.Instance.Player;
			if (_equipmentType is EquipmentType.Weapon)
			{
				foreach (PlayerWeaponData weaponData in player.Weapon.Data)
				{
					InstantiateEquipmentDisplay(weaponData);
				}
			}
			else
			{
				foreach (PlayerArmorData armorData in player.Armor.Data)
				{
					InstantiateEquipmentDisplay(armorData);
				}
			}
		}
		else
		{
			PalicoDataHolder palico = DataManager.Instance.Palico;
			if (_equipmentType is EquipmentType.Weapon)
			{
				foreach (PalicoWeaponData weaponData in palico.Weapon.Data)
				{
					InstantiateEquipmentDisplay(weaponData);
				}
			}
			else
			{
				foreach (PalicoArmorData armorData in palico.Armor.Data)
				{
					InstantiateEquipmentDisplay(armorData);
				}
			}
		}
	}

	private void ClearEntityEquipment()
	{
		Array<Node> children = _container.GetChildren();
		foreach (Node child in children)
		{
			_container.RemoveChild(child);
			child.QueueFree();
		}
	}

	private void InstantiateEquipmentDisplay(dynamic equipmentData)
	{
		EquipmentDisplay equipmentDisplay = _equipmentDisplayScene.Instantiate<EquipmentDisplay>();
		equipmentDisplay.EquipmentData = equipmentData;
		equipmentDisplay.EquipmentIcon.Texture = _weaponIcon;
		_container.AddChild(equipmentDisplay);
	}
}
