using Godot;

namespace MonsterHunterIdle;

public partial class PalicoEquipmentInfo : HBoxContainer
{
	[Export]
	private TextureRect _iconTextureRect;

	[Export]
	private CustomButton _changeEquipmentButton;

	private Palico _palico;
	private PalicoEquipmentType _equipmentType = PalicoEquipmentType.None;
	private PalicoEquipment _equipment;

	public override void _Ready()
	{
		SetEquipment();
		SetTexture();
		_changeEquipmentButton.Pressed += OnChangeEquipmentButtonPressed;
	}

	// * START - Signal Methods
	private void OnChangeEquipmentButtonPressed()
	{
		if (_equipment == null) return;
		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.ChangePalicoEquipmentButtonPressed, (int) _equipmentType);
	}
	// * END - Signal Methods

	// If the palico has equipment that corresponds to the equipment type then show to icon of that, otherwise use the default icon
	private void SetTexture()
	{
		PalicoEquipment equipment = _equipment;

		Texture2D equipmentIcon;
		if (equipment != null)
		{
			equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
		}
		else
		{
			string textureFilePath = GetTextureFilePath();
			if (textureFilePath == "") return;

			equipmentIcon = MonsterHunterIdle.GetTexture(textureFilePath);
		}

		_iconTextureRect.Texture = equipmentIcon;
	}

	private void SetEquipment()
	{
		if (_equipmentType is PalicoEquipmentType.Weapon)
		{
			_equipment = _palico.Weapon;
		}
		else if (_equipmentType is PalicoEquipmentType.Head)
		{
			_equipment = _palico.Head;
		}
		else if (_equipmentType is PalicoEquipmentType.Chest)
		{
			_equipment = _palico.Chest;
		}
	}

	private string GetTextureFilePath() => _equipmentType switch
	{
		PalicoEquipmentType.Weapon => "res://Assets/Images/Palico/PalicoWeaponWhite.png",
		PalicoEquipmentType.Head => "res://Assets/Images/Palico/PalicoHeadWhite.png",
		PalicoEquipmentType.Chest => "res://Assets/Images/Palico/PalicoChestWhite.png",
		_ => ""
	};

	public void SetPalico(Palico palico, PalicoEquipmentType equipmentType)
	{
		_palico = palico;
		_equipmentType = equipmentType;
		
		if (equipmentType is PalicoEquipmentType.Weapon)
		{
			SetEquipmentText(palico.Weapon);
		}
		else if (equipmentType is PalicoEquipmentType.Head)
		{
			SetEquipmentText(palico.Head);
		}
		else if (equipmentType is PalicoEquipmentType.Chest)
		{
			SetEquipmentText(palico.Chest);
		}
	}

	private void SetEquipmentText(PalicoEquipment equipment)
	{
		string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";
		if (equipment is PalicoWeapon weapon)
        {
            _changeEquipmentButton.Text = weapon.Tree == WeaponTree.None ? "None" : $"{equipment.Name}{subGrade}";
        }
        else if (equipment is PalicoArmor armor)
        {
            _changeEquipmentButton.Text = armor.Set == ArmorSet.None ? "None" : $"{equipment.Name}{subGrade}";
        }
	}
}
