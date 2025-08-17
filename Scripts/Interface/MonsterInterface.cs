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

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.MonsterEncountered -= OnMonsterEncountered;
		MonsterHunterIdle.Signals.LocaleChanged -= OnLocaleChanged;
		MonsterHunterIdle.Signals.MonsterLeft -= OnMonsterLeft;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.MonsterEncountered += OnMonsterEncountered;
		MonsterHunterIdle.Signals.LocaleChanged += OnLocaleChanged;
		MonsterHunterIdle.Signals.MonsterLeft += OnMonsterLeft;
	}

	public override void _Ready()
	{
		_attackButton.Pressed += OnAttackedMonster;
		_exitButton.Pressed += OnEscapedMonster;

		SetMonster(null);
	}

	private void OnMonsterEncountered(Monster monster)
	{
		string monsterMessage = PrintRich.GetMonsterMessage(monster);
		string monsterEncounteredMessage = $"You've Encountered A {monsterMessage}";
		PrintRich.PrintLine(TextColor.Orange, monsterEncounteredMessage);

		SetMonster(monster);
	}

	private void SetMonster(Monster monster)
	{
		_monster = monster;

		_monsterName.Text = monster != null ? monster.Name : "";
		_monsterIcon.Texture = monster != null ? MonsterHunterIdle.GetMonsterIcon(monster.Name) : null;
		_monsterRender.Texture = monster != null ? MonsterHunterIdle.GetMonsterRender(monster.Name) : null;

		if (monster != null)
		{
			MonsterHealthBar monsterHealthBar = MonsterHunterIdle.PackedScenes.GetMonsterHealthBar(monster);
			monsterHealthBar.Depleted += () =>
			{
				SlayedMonster();
				monsterHealthBar.QueueFree();
			};
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

		_exitButton.Visible = monster != null;
	}

	private void OnAttackedMonster()
	{
		if (_monster == null) return;

		int hunterAttack = MonsterHunterIdle.HunterManager.GetHunterAttack();

		// Console message
		string monsterMessage = PrintRich.GetMonsterMessage(_monster);
		string monsterAttackedMessage = $"The {monsterMessage} Has Been Damaged For {hunterAttack} HP";
		PrintRich.Print(TextColor.Orange, monsterAttackedMessage);

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterDamaged, hunterAttack);

		TweenDamageLabel(hunterAttack);
	}

	private void SlayedMonster()
	{
		// Console message
		string monsterMessage = PrintRich.GetMonsterMessage(_monster);
		string monsterSlayedMessage = $"The {monsterMessage} Has Been Slayed";
		PrintRich.Print(TextColor.Orange, monsterSlayedMessage); /// Don't print line | <see cref="PrintRich.PrintEncounterRewards">

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterSlayed, _monster);
		MonsterHunterIdle.MonsterManager.Encounter.GetEncounterRewards(_monster);

		SetMonster(null);
	}

	private void OnMonsterLeft(Monster monster)
	{
		// Console message
		string monsterMessage = PrintRich.GetMonsterMessage(monster);
		string monsterLeftMessage = $"The {monsterMessage} Has Left The Locale";
		PrintRich.PrintLine(TextColor.Orange, monsterLeftMessage);

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

	private void OnEscapedMonster()
	{
		// Console message
		string monsterMessage = PrintRich.GetMonsterMessage(_monster);
		string escapedMonsterMessage = $"You Escaped The {monsterMessage} Encounter";
		PrintRich.PrintLine(TextColor.Orange, escapedMonsterMessage);

		SetMonster(null);
	}

	private Label GetDamageLabel(int damage)
	{
		string redColorHex = PrintRich.GetColorHex(TextColor.Red);
		Vector2 size = new Vector2(50, 100);
		Label damageLabel = new Label()
		{
			Text = $"+ {damage}",
			Size = size,
			Position = GetLocalMousePosition() - (size / 2),
			SelfModulate = Color.FromHtml(redColorHex),
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
		};
		int fontSize = 30;
		damageLabel.AddThemeFontSizeOverride("font_size", fontSize);

		return damageLabel;
	}

	private void TweenDamageLabel(int damage)
	{
		Label damageLabel = GetDamageLabel(damage);
		AddChild(damageLabel);

		float duration = 1;
		Tween tween = CreateTween()
			.SetParallel(true)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		tween.Finished += damageLabel.QueueFree;

		Vector2 offset = new Vector2(0, -50);
		Vector2 targetPosition = damageLabel.Position + offset;
		tween.TweenProperty(damageLabel, "position", targetPosition, duration);

		Color transparent = damageLabel.SelfModulate;
		transparent.A = 0;
		tween.TweenProperty(damageLabel, "self_modulate", transparent, duration);
	}
}
