using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLog : NinePatchRect
{
	[Export] 
	private Label _resourceName;

	[Export] 
	private TextureRect _resourceIcon;

	[Export] 
	private Label _resourceAmount;

    public void UpdateResource(dynamic resource, int amount)
	{
		_resourceName.Text = $"{resource.Name}";
		_resourceIcon.Texture = resource.Icon;
		_resourceAmount.Text = $"+{amount}";
		CreateTimer();

		GD.PrintRich($"Picked Up [color=YELLOW]{resource.Name}[/color]");
	}

	private void CreateTimer()
	{
		Timer timer = new Timer()
		{
			WaitTime = 7.5f,
			Autostart = true
		};
		timer.Timeout += () => CollectionLogManager.Instance.RemoveLog(this);
		AddChild(timer);
	}
}
