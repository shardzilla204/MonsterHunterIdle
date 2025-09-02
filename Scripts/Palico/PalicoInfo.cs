using Godot;

namespace MonsterHunterIdle;

public partial class PalicoInfo : NinePatchRect
{
	[Signal]
	public delegate void LoadoutOpenedEventHandler(Palico palico);

	[Export]
	private LineEdit _palicoNameEdit;

	[Export]
	private CustomButton _palicoLoadoutButton;

	[Export]
	private Container _statContainer;

	private Palico _palico = new Palico();

    public override void _ExitTree()
    {
		MonsterHunterIdle.Signals.PalicoEquipmentChanged -= OnPalicoEquipmentChanged;
    }

    public override void _EnterTree()
    {
		MonsterHunterIdle.Signals.PalicoEquipmentChanged += OnPalicoEquipmentChanged;
    }

	public override void _Ready()
	{
		_palicoNameEdit.TextSubmitted += (string text) =>
		{
			_palicoNameEdit.ReleaseFocus();
			if (text == "")
			{
				_palicoNameEdit.Text = _palico.Name;
				return;
			}
			_palico.Name = text;
			EmitSignal(SignalName.LoadoutOpened, _palico);
		};
		_palicoNameEdit.Text = _palico.Name;
		_palicoLoadoutButton.Pressed += () => EmitSignal(SignalName.LoadoutOpened, _palico);

		RefreshStats();
	}

	// * START - Signal Methods
	private void OnPalicoEquipmentChanged(Palico palico)
	{
		if (palico != _palico) return;

		RefreshStats();
	}
	// * END - Signal Methods

	public void SetPalico(Palico palico)
	{
		_palico = palico;
	}

	private void RefreshStats()
	{
		ClearStats();

		int weaponAttack = _palico.Weapon.Name != "" ? _palico.Weapon.Attack : 0;
		HBoxContainer attackStat = Scenes.GetLoadoutStat(StatType.Attack, $"{weaponAttack}");
		_statContainer.AddChild(attackStat);

		int armorDefense = GetArmorDefense();
		HBoxContainer defenseStat = Scenes.GetLoadoutStat(StatType.Defense, $"{armorDefense}");
		_statContainer.AddChild(defenseStat);

		int weaponAffinty = _palico.Weapon.Name != "" ? _palico.Weapon.Affinity : 0;
		HBoxContainer affinityStat = Scenes.GetLoadoutStat(StatType.Affinity, $"{weaponAffinty}");
		_statContainer.AddChild(affinityStat);
		
		if (_palico.Weapon.Special != SpecialType.None && _palico.Weapon.SpecialAttack != 0)
        {
            HBoxContainer specialAttackStat = Scenes.GetLoadoutStat(_palico.Weapon.Special, $"{_palico.Weapon.SpecialAttack}");
            _statContainer.AddChild(specialAttackStat);
        }
	}

	private void ClearStats()
	{
		foreach (Node child in _statContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	private int GetArmorDefense()
	{
		int armorDefense = 0;

		if (_palico.Head.Name != "") armorDefense += _palico.Head.Defense;
		if (_palico.Chest.Name != "") armorDefense += _palico.Chest.Defense;

		return armorDefense;
	}
}
