using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class PalicoEquipmentInfo : HBoxContainer
{
	[Export]
	private PalicoEquipmentCategory _equipmentType;

	[Export]
	private TextureRect _iconTexture;

	[Export]
	private CustomButton _equipButton;

	public override void _Ready()
	{
		string textureFilePath = GetTextureFilePath();
		if (textureFilePath == "") return;

		Texture2D texture = MonsterHunterIdle.GetTexture(textureFilePath);
		_iconTexture.Texture = texture;
	}

	private string GetTextureFilePath() => _equipmentType switch
	{
		PalicoEquipmentCategory.Weapon => "res://Assets/Images/Icon/PalicoWeaponIconWhite.png",
		PalicoEquipmentCategory.Head => "res://Assets/Images/Icon/PalicoHeadEquipmentIcon.png",
		PalicoEquipmentCategory.Chest => "res://Assets/Images/Icon/PalicoChestEquipmentIcon.png",
		_ => ""
	};

	public void SetStats(Palico palico)
	{
		if (_equipmentType is PalicoEquipmentCategory.Weapon)
		{
			SetWeaponStats(palico);
		}
		else if (_equipmentType is PalicoEquipmentCategory.Head)
		{
			SetHelmetStats(palico);
		}
		else if (_equipmentType is PalicoEquipmentCategory.Chest)
		{
			SetChestStats(palico);
		}
	}

	private void SetWeaponStats(Palico palico)
	{
		_equipButton.Text = palico.Weapon is null ? "None" : palico.Weapon.Name;
	}

	private void SetHelmetStats(Palico palico)
	{
		_equipButton.Text = palico.Head is null ? "None" : palico.Head.Name;
	}

	private void SetChestStats(Palico palico)
	{
		_equipButton.Text = palico.Chest is null ? "None" : palico.Chest.Name;
	}
}
