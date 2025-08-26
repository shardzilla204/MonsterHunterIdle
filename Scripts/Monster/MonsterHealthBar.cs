using Godot;

namespace MonsterHunterIdle;

public partial class MonsterHealthBar : NinePatchRect
{
	[Signal]
	public delegate void DepletedEventHandler();

	[Export]
	private Timer _delayTimer;

	[Export]
	private NinePatchRect _damageBar;

	[Export]
	private NinePatchRect _healthBar;

	private int _monsterHealth;
	private int _maxMonsterHealth;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.MonsterDamaged -= DamageMonster;
		MonsterHunterIdle.Signals.MonsterEncounterFinished -= QueueFree;
	}

    public override void _EnterTree()
    {
		MonsterHunterIdle.Signals.MonsterDamaged += DamageMonster;
		MonsterHunterIdle.Signals.MonsterEncounterFinished += QueueFree;
    }

	public override void _Ready()
	{
		_delayTimer.Timeout += TweenDamageBar;

		_damageBar.SetAnchorsPreset(LayoutPreset.FullRect);
		_healthBar.SetAnchorsPreset(LayoutPreset.FullRect);
	}

	private void DamageMonster(int damage)
	{
		_delayTimer.Stop();

		if (_monsterHealth <= 0) return;
		
		_monsterHealth -= damage;

		float newSizeX = Size.X * ((float) _monsterHealth / _maxMonsterHealth);
		Vector2 newSize = new Vector2(newSizeX, _healthBar.Size.Y);
		_healthBar.SetDeferred("size", newSize);

		_delayTimer.Start();

		if (_monsterHealth <= 0) EmitSignal(SignalName.Depleted);
	}

	public void SetMonster(Monster monster)
	{
		_monsterHealth = monster.Health;
		_maxMonsterHealth = monster.Health;
	}

	private void TweenDamageBar()
	{
		Tween tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
		float duration = 0.5f;
		tween.TweenProperty(_damageBar, "size", _healthBar.Size /* Target Position */, duration);
	}
}
