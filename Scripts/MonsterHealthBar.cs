using Godot;

namespace MonsterHunterIdle;

public partial class MonsterHealthBar : NinePatchRect
{
	[Export]
	private TextureProgressBar _healthProgress;

    public override void _Ready()
    {
      MonsterHunterIdle.Signals.DamagedMonster += DamageMonster;
		TreeExited += () => MonsterManager.Instance.Damaged -= DamageMonster;
    }

    private void DamageMonster(int damage)
	{
		_healthProgress.Value -= damage;

		Check();
	}

	private void Check()
	{
		if (_healthProgress.Value > 0) return;

		EmitSignal(SignalName.Depleted);
	}

	public void Fill()
	{
		_healthProgress.MaxValue = MonsterManager.Instance.Encounter.Monster.Health;
		_healthProgress.Value = MonsterManager.Instance.Encounter.Monster.Health;
	}
}
