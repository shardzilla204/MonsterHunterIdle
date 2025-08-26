using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLogInterface : VBoxContainer
{
	[Export]
	private Container _collectionLogContainer;

	[Export]
	private ScrollContainer _scrollContainer;

	private bool _isHovering = false;

    public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.LocaleMaterialAdded -= OnMaterialAdded;
		MonsterHunterIdle.Signals.MonsterMaterialAdded -= OnMaterialAdded;
	}

    public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.LocaleMaterialAdded += OnMaterialAdded;
		MonsterHunterIdle.Signals.MonsterMaterialAdded += OnMaterialAdded;
	}

    public override void _Ready()
    {
		_scrollContainer.MouseEntered += () => _isHovering = true;
		_scrollContainer.MouseExited += () => _isHovering = false;
    }

	private void OnMaterialAdded(Material material)
	{
		if (_isHovering) return;

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
