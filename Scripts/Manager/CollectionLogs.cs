using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLogContainer : Container
{
	private List<CollectionLog> _collectionLogs = new List<CollectionLog>();

	public void AddCollectionLog(dynamic material, int quantity)
	{
		CollectionLog collectionLog = MonsterHunterIdle.PackedScenes.GetCollectionLog();
		collectionLog.SetResource(material, quantity);

		AddChild(collectionLog);

		AddMaterial(material, quantity);
	}

	public void RemoveCollectionLog(CollectionLog collectionLog)
	{
		Color targetColor = Colors.White;
		targetColor.A = 0f; 

		Tween tween = CreateTween();
		tween.TweenProperty(collectionLog, "modulate", targetColor, 1f);
		tween.Finished += collectionLog.QueueFree;
	}

	private void AddMaterial(dynamic material, int quantity)
	{
		for (int i = 0; i < quantity; i++)
		{
			GameManager.Instance.ItemBox.Materials.Add(material);
		}
	}
}
