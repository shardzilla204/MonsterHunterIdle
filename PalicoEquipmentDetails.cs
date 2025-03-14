using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class PalicoEquipmentDetails : HBoxContainer
{
	[Export]
	private PalicoEquipmentType _equipmentType;

	[Export]
	private Texture2D _weaponTexture;

	[Export]
	private Texture2D _helmetTexture;

	[Export]
	private Texture2D _armorTexture;

	[Export]
	private TextureRect _iconTexture;

	[Export]
	private EquipButton _equipButton;

	[Export]
	private PackedScene _statDetailScene;

	[Export]
	private Container _statsContainer;

	public override void _Ready()
	{
		ClearStats();
		_iconTexture.Texture = GetTexture();
	}

	private Texture2D GetTexture() => _equipmentType switch
	{
		PalicoEquipmentType.Weapon => _weaponTexture,
		PalicoEquipmentType.Helmet => _helmetTexture,
		PalicoEquipmentType.Armor => _armorTexture,
		_ => _weaponTexture
	};

	public void GetStats(PalicoData palico)
	{
		if (_equipmentType is PalicoEquipmentType.Weapon)
		{
			GetWeaponStats(palico);
		}
		else if (_equipmentType is PalicoEquipmentType.Helmet)
		{
			GetHelmetStats(palico);
		}
		else if (_equipmentType is PalicoEquipmentType.Armor)
		{
			GetArmorStats(palico);
		}
	}

	private void GetWeaponStats(PalicoData palico)
	{
		_equipButton.Text = palico.Weapon is null ? "None" : palico.Weapon.Name;

		if (palico.Weapon is null) return;

		List<EquipmentStat> equipmentStats = GetWeaponEquipmentStats(palico.Weapon);

		for (int i = 0; i < equipmentStats.Count; i++)
		{
			if (equipmentStats[i].Value == 0) continue;

			SetStat(equipmentStats[i].StatType, equipmentStats[i].Value);
		}
	}

	private List<EquipmentStat> GetWeaponEquipmentStats(PalicoWeaponData palicoWeapon)
	{
		return new List<EquipmentStat>()
		{
			new EquipmentStat(StatType.Attack, palicoWeapon.Attack),
			new EquipmentStat(StatType.Defense, palicoWeapon.Defense),
			new EquipmentStat(StatType.Affinity, palicoWeapon.Affinity)
		};
	}

	private void GetHelmetStats(PalicoData palico)
	{
		_equipButton.Text = palico.Helmet is null ? "None" : palico.Helmet.Name;

		if (palico.Helmet is null) return;

		List<EquipmentStat> equipmentStats = GetArmorEquipmentStats(palico.Helmet);

		for (int i = 0; i < equipmentStats.Count; i++)
		{
			if (equipmentStats[i].Value == 0) continue;

			SetStat(equipmentStats[i].StatType, equipmentStats[i].Value);
		}
	}

	private void GetArmorStats(PalicoData palico)
	{
		_equipButton.Text = palico.Armor is null ? "None" : palico.Armor.Name;

		if (palico.Armor is null) return;

		List<EquipmentStat> equipmentStats = GetArmorEquipmentStats(palico.Armor);

		for (int i = 0; i < equipmentStats.Count; i++)
		{
			if (equipmentStats[i].Value == 0) continue;

			SetStat(equipmentStats[i].StatType, equipmentStats[i].Value);
		}
	}

	private List<EquipmentStat> GetArmorEquipmentStats(PalicoArmorData palicoArmor)
	{
		return new List<EquipmentStat>()
		{
			new EquipmentStat(StatType.Defense, palicoArmor.Defense),
			new EquipmentStat(StatType.Fire, palicoArmor.Resistances[0].Value),
			new EquipmentStat(StatType.Water, palicoArmor.Resistances[1].Value),
			new EquipmentStat(StatType.Thunder, palicoArmor.Resistances[2].Value),
			new EquipmentStat(StatType.Ice, palicoArmor.Resistances[3].Value),
			new EquipmentStat(StatType.Dragon, palicoArmor.Resistances[4].Value),
			new EquipmentStat(StatType.Health, palicoArmor.Health),
		};
	}

	private void SetStat(StatType statType, int value)
	{
		StatDetail statDetail = _statDetailScene.Instantiate<StatDetail>();
		statDetail.FillPalicoStat(statType, $"+{value}");
		_statsContainer.AddChild(statDetail);
	}

	private void ClearStats()
	{
		foreach (Node child in _statsContainer.GetChildren())
		{
			_statsContainer.RemoveChild(child);
			child.QueueFree();
		}
	}
}

public class EquipmentStat
{
	public EquipmentStat(StatType statType, int value)
	{
		StatType = statType;
		Value = value;
	}

	public StatType StatType;
	public int Value;
}
