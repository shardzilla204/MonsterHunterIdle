using Godot;

namespace MonsterHunterIdle;

public partial class PackedScenes : Node
{
   [Export]
	private PackedScene _statDetail;

   [Export]
	private PackedScene _monsterInterface;

	[Export]
	private PackedScene _collectionLogInterface;

	[Export]
	private PackedScene _itemBoxInterface;

	[Export]
	private PackedScene _smithyInterface;

	[Export]
	private PackedScene _playerInterface;

	[Export]
	private PackedScene _palicoInterface;

   [Export]
   private PackedScene _palicoLoadout;

   [Export] 
	private PackedScene _collectionLog;

   public StatDetail GetStatDetail()
   {
      return _statDetail.Instantiate<StatDetail>();
   }

   public MonsterInterface GetMonsterInterface()
   {
      return _monsterInterface.Instantiate<MonsterInterface>();
   }

   public CollectionLogInterface GetCollectionLogInterface()
   {
      return _collectionLogInterface.Instantiate<CollectionLogInterface>();
   }

   public ItemBoxInterface GetItemBoxInterface()
   {
      return _itemBoxInterface.Instantiate<ItemBoxInterface>();
   }

   public SmithyInterface GetSmithyInterface()
   {
      return _smithyInterface.Instantiate<SmithyInterface>();
   }

   public PlayerInterface GetPlayerInterface()
   {
      return _playerInterface.Instantiate<PlayerInterface>();
   }

   public PalicoInterface GetPalicoInterface()
   {
      return _palicoInterface.Instantiate<PalicoInterface>();
   }

   public CollectionLog GetCollectionLog()
   {
      return _collectionLog.Instantiate<CollectionLog>();
   }

   public PalicoLoadout GetPalicoLoadout()
   {
      return _palicoLoadout.Instantiate<PalicoLoadout>();
   }
}