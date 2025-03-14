using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLogInterface : VBoxContainer
{
	[Export]
	private Container _collectionLogContainer;

	[Export]
	private ScrollContainer _scrollContainer;

	public override void _Ready()
	{
		// CollectionLogManager.Instance.CollectionLogContainer = _collectionLogContainer;
	}

    public override void _Process(double delta)
    {
        _scrollContainer.EnsureControlVisible(_collectionLogContainer);
    }

	private void ClearDisplay()
	{
		foreach (Node collectionLog in _collectionLogContainer.GetChildren())
		{
			_collectionLogContainer.RemoveChild(collectionLog);
			collectionLog.QueueFree();
		}
	}
}
