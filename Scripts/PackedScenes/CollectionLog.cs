using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLog : Control
{
	[Export]
	private NinePatchRect _ninePatchRect;

	[Export]
	private Label _materialName;

	[Export]
	private TextureRect _materialIcon;

	public override void _Ready()
	{
		Tween();
	}

	public void SetMaterial(Material material)
	{
		_materialName.Text = material.Name;
		_materialIcon.Texture = MonsterHunterIdle.GetMaterialIcon(material);

		CreateTimer();
	}

	private void CreateTimer()
	{
		Timer timer = new Timer()
		{
			WaitTime = 7.5f,
			Autostart = true
		};
		timer.Timeout += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.CollectionLogTimedOut, this);
		AddChild(timer);
	}

	private void Tween()
	{
		int positionY = 100;
		Vector2 belowPosition = Position + new Vector2(0, positionY);
		_ninePatchRect.Position = belowPosition;

		Color transparent = Colors.White;
		transparent.A = 0;
		_ninePatchRect.Modulate = transparent;

		float duration = 2;
		Tween tween = CreateTween().SetParallel(true).SetEase(Godot.Tween.EaseType.InOut).SetTrans(Godot.Tween.TransitionType.Spring);
		tween.TweenProperty(_ninePatchRect, "position", Position, duration / 2);
		tween.TweenProperty(_ninePatchRect, "modulate", Colors.White, duration);
	}
}
