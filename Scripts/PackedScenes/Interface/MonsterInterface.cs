using Godot;

namespace MonsterHunterIdle;

public partial class MonsterInterface : NinePatchRect
{
	[Export]
	private Label _monsterName;

	[Export]
	private TextureRect _monsterIcon;

	[Export]
	private TextureRect _monsterRender;

	[Export]
	private Container _monsterInformation;

	[Export]
	private StarContainer _starContainer;

	[Export]
	private Button _attackButton;

	[Export]
	private CustomButton _exitButton;

	[Export]
	private MonsterTimer _monsterTimer;

	private Monster _monster;
	private Vector2 _renderPosition;
	private ChargeBarTimer _chargeBarTimer;

	private bool _isAttackButtonPressed = false;
	private bool _isHovering = false;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.MonsterEncountered -= OnMonsterEncountered;
		MonsterHunterIdle.Signals.LocaleChanged -= OnLocaleChanged;
		MonsterHunterIdle.Signals.MonsterLeft -= OnMonsterLeft;
		// MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.MonsterEncountered += OnMonsterEncountered;
		MonsterHunterIdle.Signals.LocaleChanged += OnLocaleChanged;
		MonsterHunterIdle.Signals.MonsterLeft += OnMonsterLeft;
		// MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
	}

	public override void _Ready()
	{
		_attackButton.MouseEntered += () => _isHovering = true;
		_attackButton.MouseExited += () => _isHovering = false;

		_attackButton.Pressed += OnAttackedMonster;
		_exitButton.Pressed += OnEscapedMonster;

		_renderPosition = _monsterRender.Position;

		SetMonster(null);
	}

	public override void _Process(double delta)
	{
		if (!_isHovering) return;

		_isAttackButtonPressed = Input.IsMouseButtonPressed(MouseButton.Left);
	}

	// * START - Signal Methods
	private void OnMonsterEncountered(Monster monster)
	{
		string monsterMessage = PrintRich.GetMonsterMessage(monster);
		string monsterEncounteredMessage = $"You've Encountered A {monsterMessage}";
		PrintRich.PrintLine(TextColor.Orange, monsterEncounteredMessage);

		SetMonster(monster);
	}

	private async void OnMonsterLeft()
	{
		// Console message
		string monsterMessage = PrintRich.GetMonsterMessage(_monster);
		string monsterLeftMessage = $"The {monsterMessage} Has Left The Locale";
		PrintRich.PrintLine(TextColor.Orange, monsterLeftMessage);

		Tween tweenLeave = TweenLeave();
		await ToSignal(tweenLeave, Tween.SignalName.Finished);

		SetMonster(null);
	}

	private void OnLocaleChanged()
	{
		if (_monster is null) return;

		// Console message
		string monsterMessage = PrintRich.GetMonsterMessage(_monster);
		string leftMonsterMessage = $"You Left The {monsterMessage} Encounter In The Locale";
		PrintRich.PrintLine(TextColor.Orange, leftMonsterMessage);

		SetMonster(null);
	}

	private async void OnEscapedMonster()
	{
		// Console message
		string monsterMessage = PrintRich.GetMonsterMessage(_monster);
		string escapedMonsterMessage = $"You Escaped The {monsterMessage} Encounter";
		PrintRich.PrintLine(TextColor.Orange, escapedMonsterMessage);

		Tween tweenEscape = TweenEscape();
		await ToSignal(tweenEscape, Tween.SignalName.Finished);

		SetMonster(null);
	}

	private void OnHealthBarDepleted()
	{
		// Console message
		string monsterMessage = PrintRich.GetMonsterMessage(_monster);
		string monsterSlayedMessage = $"The {monsterMessage} Has Been Slayed";
		PrintRich.Print(TextColor.Orange, monsterSlayedMessage); /// Don't print line | <see cref="PrintRich.PrintEncounterRewards">

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterSlayed, _monster);
		MonsterManager.Encounter.GetEncounterRewards(_monster);

		SetMonster(null, true);
	}

	private async void OnAttackedMonster()
	{
		if (_monster == null || IsInstanceValid(_chargeBarTimer)) return;

		while (_isAttackButtonPressed)
		{
			// ? Add charge first & once it's done, attack
			float weaponChargeTime = EquipmentManager.GetWeaponChargeTime();
			_chargeBarTimer = MonsterHunterIdle.PackedScenes.GetChargeBarTimer(weaponChargeTime);
			_chargeBarTimer.TreeExited += () => _chargeBarTimer = null;

			AddChild(_chargeBarTimer);

			await ToSignal(_chargeBarTimer, ChargeBarTimer.SignalName.ChargingFinished);

			int hunterAttack = EquipmentManager.GetWeaponAttack();
			int hunterSpecialAttack = EquipmentManager.GetWeaponSpecialAttack();

			bool hasHitWeakness = HasHitWeakness();
			if (hasHitWeakness && hunterSpecialAttack != 0)
			{
				// Add weakness damage
				float bonusPercentage = 1.75f;
				hunterSpecialAttack = Mathf.RoundToInt(hunterSpecialAttack * bonusPercentage);
			}

			int totalDamage = hunterAttack + hunterSpecialAttack;

			// Console message
			string monsterMessage = PrintRich.GetMonsterMessage(_monster);
			string attackMessage = hunterSpecialAttack == 0 ? $"{hunterAttack}" : $"{hunterAttack} + {hunterSpecialAttack} ({Hunter.Weapon.Special})";
			string monsterAttackedMessage = $"The {monsterMessage} Has Been Damaged For {attackMessage} | {totalDamage} HP";
			PrintRich.Print(TextColor.Orange, monsterAttackedMessage);

			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterDamaged, totalDamage);

			Label attackNode = Scenes.GetDamageLabel(this, hunterAttack, TextColor.Red);
			TweenNode(attackNode);

			if (hunterSpecialAttack == 0 || Hunter.Weapon.Special == SpecialType.None) return;

			HBoxContainer specialAttackContainer = Scenes.GetSpecialAttackContainer(this, hunterSpecialAttack);
			TweenNode(specialAttackContainer);
		}
	}

	private void OnInterfaceChanged(InterfaceType interfaceType)
	{
		QueueFree();
	}
	// * END - Signal Methods

	private async void SetMonster(Monster monster, bool hasSlayed = false)
	{
		/// For showing tweens | <see cref="TweenEncounter"> <see cref="TweenLeave"> <see cref="TweenEscape">
		if (!hasSlayed)
		{
			_monsterRender.Texture = monster != null ? MonsterHunterIdle.GetMonsterRender(monster.Name) : null;
		}

		_monster = monster;

		_monsterName.Text = monster != null ? monster.Name : "";
		_monsterIcon.Texture = monster != null ? MonsterHunterIdle.GetMonsterIcon(monster.Name) : null;

		_monsterRender.PivotOffset = _monsterRender.Size / 2;
		_exitButton.Visible = monster != null;

		// For encountering a monster or finishing an encounter
		if (monster != null)
		{
			Tween tweenEncounter = TweenEncounter();
			await ToSignal(tweenEncounter, Tween.SignalName.Finished);

			MonsterHealthBar monsterHealthBar = MonsterHunterIdle.PackedScenes.GetMonsterHealthBar(monster);
			monsterHealthBar.Depleted += OnHealthBarDepleted;
			_monsterInformation.AddChild(monsterHealthBar);
			_monsterTimer.Start();
			_starContainer.Fill(monster);
		}
		else
		{
			_monsterTimer.Stop();
			_starContainer.Empty();

			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterEncounterFinished);
		}

		/// For showing tween | <see cref="TweenSlayed">
		if (hasSlayed)
		{
			Tween tweenSlayed = TweenSlayed();
			await ToSignal(tweenSlayed, Tween.SignalName.Finished);

			_monsterRender.Texture = monster != null ? MonsterHunterIdle.GetMonsterRender(monster.Name) : null;
		}
	}

	private bool HasHitWeakness()
	{
		SpecialType weaponSpecialType = Hunter.Weapon.Special;
		bool hasHitWeakness = _monster.SpecialWeaknesses.Contains(weaponSpecialType);

		return hasHitWeakness;
	}

	// * START - Tween Methods
	private void TweenNode(Control node)
	{
		AddChild(node);

		float duration = 1;
		Tween tween = CreateTween()
			.SetParallel(true)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		tween.Finished += node.QueueFree;

		RandomNumberGenerator RNG = new RandomNumberGenerator();
		int offsetX = RNG.RandiRange(-50, 50);
		int offsetY = -50;
		Vector2 targetPosition = node.Position + new Vector2(offsetX, offsetY);
		tween.TweenProperty(node, "position", targetPosition, duration);

		Color transparent = node.SelfModulate;
		transparent.A = 0;
		tween.TweenProperty(node, "self_modulate", transparent, duration);
	}

	// No need to reset to original state as the original state is the target
	private Tween TweenEncounter()
	{
		int offsetX = (int)Size.X;
		Vector2 startPosition = _monsterRender.Position + new Vector2(offsetX, 0);
		_monsterRender.Position = startPosition;

		float duration = 0.5f;
		Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(_monsterRender, "position", _renderPosition, duration);

		return tween;
	}

	private Tween TweenLeave()
	{
		int offsetX = (int)-Size.X;
		Vector2 targetPosition = _monsterRender.Position + new Vector2(offsetX, _monsterRender.Position.Y);

		float duration = 0.5f;
		Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(_monsterRender, "position", targetPosition, duration);
		tween.Finished += () =>
		{
			// Reset all properties to original state
			_monsterRender.Position = _renderPosition;
		};
		return tween;
	}

	private Tween TweenEscape()
	{
		Vector2 startScale = _monsterRender.Scale;

		float scaleValue = 0.25f;
		Vector2 targetScale = new Vector2(scaleValue, scaleValue);

		Color transparent = Colors.White;
		transparent.A = 0;

		float duration = 0.5f;
		Tween tween = CreateTween().SetParallel(true).SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(_monsterRender, "scale", targetScale, duration);
		tween.TweenProperty(_monsterRender, "self_modulate", transparent, duration);
		tween.Finished += () =>
		{
			// Reset all properties to original state
			_monsterRender.Scale = startScale;
			_monsterRender.SelfModulate = Colors.White;
		};
		return tween;
	}

	private Tween TweenSlayed()
	{
		float startRotation = _monsterRender.Rotation;

		int offsetY = (int)Size.Y;
		Vector2 targetPosition = _monsterRender.Position + new Vector2(_monsterRender.Position.X, offsetY);

		float targetRotation = Mathf.DegToRad(-15);

		Color transparent = Colors.White;
		transparent.A = 0;

		float duration = 1;
		Tween tween = CreateTween().SetParallel(true);
		tween.TweenProperty(_monsterRender, "position", targetPosition, duration);
		tween.TweenProperty(_monsterRender, "rotation", targetRotation, duration / 2);
		tween.TweenProperty(_monsterRender, "self_modulate", transparent, duration);
		tween.Finished += () =>
		{
			// Reset all properties to original state
			_monsterRender.Position = _renderPosition;
			_monsterRender.Rotation = startRotation;
			_monsterRender.SelfModulate = Colors.White;
		};

		return tween;
	}
	// * END - Tween Methods
}
