// using System.Collections.Generic;
// using Godot;

// namespace MonsterHunterIdle;

// public partial class PalicoEquipmentDetails : HBoxContainer
// {
// 	[Export]
// 	private PalicoEquipmentType _equipmentType;

// 	[Export]
// 	private TextureRect _iconTexture;

// 	[Export]
// 	private EquipButton _equipButton;

// 	[Export]
// 	private Container _statsContainer;

// 	public override void _Ready()
// 	{
// 		ClearStats();
// 		// _iconTexture.Texture = GetTexture();
// 	}

// 	// private Texture2D GetTexture() => _equipmentType switch
// 	// {
// 	// 	PalicoEquipmentType.Weapon => _weaponTexture,
// 	// 	PalicoEquipmentType.Helmet => _helmetTexture,
// 	// 	PalicoEquipmentType.Armor => _armorTexture,
// 	// 	_ => _weaponTexture
// 	// };

// 	public void SetStats(Palico palico)
// 	{
// 		if (_equipmentType is PalicoEquipmentType.Weapon)
// 		{
// 			SetWeaponStats(palico);
// 		}
// 		else if (_equipmentType is PalicoEquipmentType.Helmet)
// 		{
// 			SetHelmetStats(palico);
// 		}
// 		else if (_equipmentType is PalicoEquipmentType.Chest)
// 		{
// 			SetChestStats(palico);
// 		}
// 	}

// 	private void SetWeaponStats(Palico palico)
// 	{
// 		_equipButton.Text = palico.Weapon is null ? "None" : palico.Weapon.Name;

// 		if (palico.Weapon is null) return;

// 		List<PalicoEquipmentStat> equipmentStats = GetWeaponPalicoEquipmentStats(palico.Weapon);

// 		for (int i = 0; i < equipmentStats.Count; i++)
// 		{
// 			if (equipmentStats[i].Value == 0) continue;

// 			SetStat(equipmentStats[i].StatType, equipmentStats[i].Value);
// 		}
// 	}

// 	private List<PalicoEquipmentStat> GetWeaponPalicoEquipmentStats(Weapon palicoWeapon)
// 	{
// 		// return new List<PalicoEquipmentStat>()
// 		// {
// 		// 	new PalicoEquipmentStat(StatType.Attack, palicoWeapon.Attack),
// 		// 	new PalicoEquipmentStat(StatType.Defense, palicoWeapon.Defense),
// 		// 	new PalicoEquipmentStat(StatType.Affinity, palicoWeapon.Affinity)
// 		// };
// 	}

// 	private void SetHelmetStats(Palico palico)
// 	{
// 		_equipButton.Text = palico.Helmet is null ? "None" : palico.Helmet.Name;

// 		if (palico.Helmet is null) return;

// 		// List<PalicoEquipmentStat> equipmentStats = GetPalicoEquipmentStats(palico.Helmet);

// 		for (int i = 0; i < equipmentStats.Count; i++)
// 		{
// 			if (equipmentStats[i].Value == 0) continue;

// 			SetStat(equipmentStats[i].StatType, equipmentStats[i].Value);
// 		}
// 	}

// 	private void SetChestStats(Palico palico)
// 	{
// 		_equipButton.Text = palico.Chest is null ? "None" : palico.Chest.Name;

// 		if (palico.Chest is null) return;

// 		List<PalicoEquipmentStat> equipmentStats = GetPalicoEquipmentStats(palico.Chest);

// 		for (int i = 0; i < equipmentStats.Count; i++)
// 		{
// 			if (equipmentStats[i].Value == 0) continue;

// 			SetStat(equipmentStats[i].StatType, equipmentStats[i].Value);
// 		}
// 	}

// 	private void SetStat(StatType statType, int value)
// 	{
// 		StatDetail statDetail = MonsterHunterIdle.PackedScenes.GetStatDetail();
// 		statDetail.FillPalicoStat(statType, $"+{value}");
// 		_statsContainer.AddChild(statDetail);
// 	}

// 	private void ClearStats()
// 	{
// 		foreach (Node child in _statsContainer.GetChildren())
// 		{
// 			_statsContainer.RemoveChild(child);
// 			child.QueueFree();
// 		}
// 	}
// }
