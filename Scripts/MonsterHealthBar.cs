using Godot;

namespace MonsterHunterIdle;

public partial class MonsterHealthBar : NinePatchRect
{
	[Signal]
	public delegate void DepletedEventHandler();

	[Export]
	private TextureProgressBar _healthProgress;

	public void DamageMonster(int damage)
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
		_healthProgress.MaxValue = MonsterManager.Instance.Monster.Health;
		_healthProgress.Value = MonsterManager.Instance.Monster.Health;
	}
}
