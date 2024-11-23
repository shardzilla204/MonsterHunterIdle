using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLogDisplay : VBoxContainer
{
	[Export]
	private GatherButton _gatherButton;

	[Export]
	private Container _collectionLogContainer;

	[Export]
	private ScrollContainer _scrollContainer;

	public override void _Ready()
	{
		CollectionLogManager.Instance.CollectionLogContainer = _collectionLogContainer;
		_gatherButton.Finished += () =>
		{
			dynamic material = BiomeManager.Instance.GetBiomeMaterial();
			material.Quantity++;
			CollectionLogManager.Instance.AddLog(material, 1);
		};
	}

    public override void _Process(double delta)
    {
        _scrollContainer.EnsureControlVisible(_collectionLogContainer);
    }
}
