using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class CollectionLogManager : Node
{
	private static CollectionLogManager _instance;
	public static CollectionLogManager Instance
	{
		get => _instance;
		private set 
		{
			if (_instance == null)
			{
				_instance = value;
			}
			else if (_instance != value)
			{
				GD.PrintRich($"{nameof(CollectionLogManager)} already exists");
			}
		}
	}

	[Export] 
	private PackedScene _collectionLogScene;

	public Container CollectionLogContainer;
	
	private List<CollectionLog> _collectionLogs = new List<CollectionLog>();

	public override void _Ready()
	{
		Instance = this;
	}

	public void AddLog(dynamic resource, int amount)
	{
		CollectionLog collectionLog = _collectionLogScene.Instantiate<CollectionLog>();
		collectionLog.UpdateResource(resource, amount);
		CollectionLogContainer.AddChild(collectionLog);
		AddMaterial(resource, amount);
	}

	private void AddMaterial(dynamic resource, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			GameManager.Instance.ItemBox.Materials.Add(resource);
		}
	}

	public void RemoveLog(CollectionLog collectionLog)
	{
		TweenRemoveLog(collectionLog);
	}

	private void TweenRemoveLog(CollectionLog collectionLog)
	{
		Color targetColor = Colors.White;
		targetColor.A = 0f; 
		Tween tween = CreateTween();
		tween.TweenProperty(collectionLog, "modulate", targetColor, 1f);
		tween.Finished += collectionLog.QueueFree;
	}

	public void Clear()
	{
		foreach (CollectionLog collectionLog in _collectionLogs)
		{
			_collectionLogs.Remove(collectionLog);
			collectionLog.QueueFree();
		}
	}
}
