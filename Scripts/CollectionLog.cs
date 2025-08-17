using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLog : NinePatchRect
{
	[Export] 
	private Label _materialName;

	[Export] 
	private TextureRect _materialIcon;

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
}
