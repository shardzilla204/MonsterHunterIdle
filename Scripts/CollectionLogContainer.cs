using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLogContainer : Container
{
    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.CollectionLogTimedOut -= OnCollectionLogTimedOut;
		MonsterHunterIdle.Signals.LocaleMaterialAdded -= OnMaterialAdded;
		MonsterHunterIdle.Signals.MonsterMaterialAdded -= OnMaterialAdded;
    }

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.CollectionLogTimedOut += OnCollectionLogTimedOut;
		MonsterHunterIdle.Signals.LocaleMaterialAdded += OnMaterialAdded;
		MonsterHunterIdle.Signals.MonsterMaterialAdded += OnMaterialAdded;
	}

	public void OnMaterialAdded(Material material)
	{
		CollectionLog collectionLog = MonsterHunterIdle.PackedScenes.GetCollectionLog();
		collectionLog.SetMaterial(material);

		AddChild(collectionLog);
	}

	private void OnCollectionLogTimedOut(CollectionLog collectionLog)
	{
		Color targetColor = Colors.White;
		targetColor.A = 0f; 

		Tween tween = CreateTween();
		tween.TweenProperty(collectionLog, "modulate", targetColor, 1f);
		tween.Finished += collectionLog.QueueFree;
	}

}
