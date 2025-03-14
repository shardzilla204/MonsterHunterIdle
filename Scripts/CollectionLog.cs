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

    public void SetResource(dynamic resource, int amount)
	{
		_resourceName.Text = resource.Name;
		_resourceIcon.Texture = resource.Icon;
		_resourceAmount.Text = $"+{amount}";

		CreateTimer();
	}

	private void CreateTimer()
	{
		Timer timer = new Timer()
		{
			WaitTime = 7.5f,
			Autostart = true
		};
		timer.Timeout += () => CollectionLogManager.Instance.RemoveCollectionLog(this);
		AddChild(timer);
	}
}
