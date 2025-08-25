using System;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class PackedScenes : Node
{
   [Export]
   private PackedScene _statDetail;

   [Export]
   private PackedScene _palicoLoadout;

   [Export]
   private PackedScene _collectionLog;

   [Export]
   private PackedScene _equipmentButton;

   [Export]
   private PackedScene _palicoDetails;

   [Export]
   private PackedScene _materialLog;

   [Export]
   private PackedScene _craftingMaterialLog;

   [Export]
   private PackedScene _monsterHealthBar;

   [Export]
   private PackedScene _equipmentOptionButton;

   [Export]
   private PackedScene _equipmentInfo;

   [Export]
   private PackedScene _sellMaterialLogContainer;

   [Export]
   private PackedScene _sellMaterialLog;

   [Export]
   private PackedScene _craftingFilter;

   [ExportGroup("Interfaces")]
   [Export]
   private PackedScene _equipmentSelectionInterface;

   [Export]
   private PackedScene _equipmentOptionInfoInterface;

   [Export]
   private PackedScene _monsterInterface;

   [Export]
   private PackedScene _collectionLogInterface;

   [Export]
   private PackedScene _itemBoxInterface;

   [Export]
   private PackedScene _craftingFilterInterface;

   [Export]
   private PackedScene _recipeInterface;

   [Export]
   private PackedScene _loadoutInterface;

   [Export]
   private PackedScene _smithyInterface;

   [Export]
   private PackedScene _playerInterface;

   [Export]
   private PackedScene _palicoInterface;

   [Export]
   private PackedScene _changeEquipmentInterface;

   [Export]
   private PackedScene _settingsInterface;

   [Export]
   private PackedScene _offlineInterface;

   public override void _EnterTree()
   {
      MonsterHunterIdle.PackedScenes = this;
   }

   public StatDetail GetStatDetail()
   {
      return _statDetail.Instantiate<StatDetail>();
   }

   public SellMaterialLog GetSellMaterialLog(Material material)
   {
      SellMaterialLog sellMaterialLog = _sellMaterialLog.Instantiate<SellMaterialLog>();
      sellMaterialLog.SetMaterial(material);
      return sellMaterialLog;
   }

   public SellMaterialLogContainer GetSellMaterialLogContainer()
   {
      return _sellMaterialLogContainer.Instantiate<SellMaterialLogContainer>();
   }

   public EquipmentInfo GetEquipmentInfo(Equipment equipment)
   {
      EquipmentInfo equipmentInfo = _equipmentInfo.Instantiate<EquipmentInfo>();
      equipmentInfo.SetEquipment(equipment);
      return equipmentInfo;
   }

   public EquipmentOptionButton GetEquipmentOptionButton(Equipment equipment)
   {
      EquipmentOptionButton equipmentOptionButton = _equipmentOptionButton.Instantiate<EquipmentOptionButton>();
      equipmentOptionButton.SetEquipment(equipment);
      return equipmentOptionButton;
   }

   public CraftingMaterialLog GetCraftingMaterialLog(Material material, int amount)
   {
      CraftingMaterialLog craftingMaterialLog = _craftingMaterialLog.Instantiate<CraftingMaterialLog>();
      craftingMaterialLog.SetMaterial(material, amount);
      return craftingMaterialLog;
   }

   public CollectionLog GetCollectionLog()
   {
      return _collectionLog.Instantiate<CollectionLog>();
   }

   public PalicoLoadout GetPalicoLoadout(Palico palico)
   {
      PalicoLoadout palicoLoadout = _palicoLoadout.Instantiate<PalicoLoadout>();
      palicoLoadout.SetPalico(palico);
      return palicoLoadout;
   }

   public MonsterHealthBar GetMonsterHealthBar(Monster monster)
   {
      MonsterHealthBar monsterHealthBar = _monsterHealthBar.Instantiate<MonsterHealthBar>();
      monsterHealthBar.SetMonster(monster);

      return monsterHealthBar;
   }

   public PalicoDetails GetPalicoDetails()
   {
      return _palicoDetails.Instantiate<PalicoDetails>();
   }

   public CraftButton GetCraftButton(Equipment equipment)
   {
      CraftButton craftButton = _equipmentButton.Instantiate<CraftButton>();
      craftButton.SetEquipment(equipment);
      return craftButton;
   }

   public MaterialLog GetMaterialLog(Material material, int amount = 0)
   {
      MaterialLog materialLog = _materialLog.Instantiate<MaterialLog>();
      materialLog.SetMaterial(material, amount);
      return materialLog;
   }

   public CraftingFilter GetCraftingFilter(Enum category)
   {
      string fileName = category.ToString();

      CraftingFilter craftingFilter = _craftingFilter.Instantiate<CraftingFilter>();
      craftingFilter.SetTexture(fileName);
      craftingFilter.Category = category;
      return craftingFilter;
   }

   // * Interface scenes
   public RecipeInterface GetRecipeInterface(Equipment equipment)
   {
      RecipeInterface recipeInterface = _recipeInterface.Instantiate<RecipeInterface>();
      recipeInterface.SetMaterials(equipment);
      return recipeInterface;
   }

   public ChangeEquipmentInterface GetChangeEquipmentInterface(Equipment equipment)
   {
      ChangeEquipmentInterface changeEquipmentInterface = _changeEquipmentInterface.Instantiate<ChangeEquipmentInterface>();
      changeEquipmentInterface.SetEquipment(equipment);
      return changeEquipmentInterface;
   }

   public EquipmentSelectionInterface GetEquipmentSelectionInterface(Equipment equipment)
   {
      EquipmentSelectionInterface equipmentSelectionInterface = _equipmentSelectionInterface.Instantiate<EquipmentSelectionInterface>();
      equipmentSelectionInterface.SetEquipment(equipment);
      return equipmentSelectionInterface;
   }

   public ItemBoxInterface GetItemBoxInterface()
   {
      return _itemBoxInterface.Instantiate<ItemBoxInterface>();
   }

   public EquipmentOptionInfoInterface GetEquipmentOptionInfoInterface(Equipment equipment)
   {
      EquipmentOptionInfoInterface equipmentOptionInfoInterface = _equipmentOptionInfoInterface.Instantiate<EquipmentOptionInfoInterface>();
      equipmentOptionInfoInterface.SetEquipment(equipment);
      return equipmentOptionInfoInterface;
   }

   public SettingsInterface GetSettingsInterface()
   {
      return _settingsInterface.Instantiate<SettingsInterface>();
   }

   public SmithyInterface GetSmithyInterface()
   {
      return _smithyInterface.Instantiate<SmithyInterface>();
   }

   public HunterInterface GetHunterInterface()
   {
      return _playerInterface.Instantiate<HunterInterface>();
   }

   public VBoxContainer GetLoadoutInterface()
   {
      return _loadoutInterface.Instantiate<VBoxContainer>();
   }

   public PalicoInterface GetPalicoInterface()
   {
      return _palicoInterface.Instantiate<PalicoInterface>();
   }

   public CollectionLogInterface GetCollectionLogInterface()
   {
      return _collectionLogInterface.Instantiate<CollectionLogInterface>();
   }

   public MonsterInterface GetMonsterInterface()
   {
      return _monsterInterface.Instantiate<MonsterInterface>();
   }

   public CraftingFilterInterface GetCraftingFilterInterface()
   {
      return _craftingFilterInterface.Instantiate<CraftingFilterInterface>();
   }

   public OfflineInterface GetOfflineInterface(Dictionary<string, int> timeDifference)
   {
      OfflineInterface offlineInterface = _offlineInterface.Instantiate<OfflineInterface>();
      offlineInterface.SetTime(timeDifference);
      return offlineInterface;
   }
}