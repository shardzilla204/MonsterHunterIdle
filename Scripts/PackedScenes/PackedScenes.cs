using System;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class PackedScenes : Node
{
   [Export]
   private PackedScene _materialLog;

   [Export]
   private PackedScene _craftingMaterialLog;

   [Export]
   private PackedScene _monsterHealthBar;

   [Export]
   private PackedScene _sellMaterialLogContainer;

   [Export]
   private PackedScene _sellMaterialLog;

   [Export]
   private PackedScene _craftingFilter;

   [Export]
   private PackedScene _chargeBarTimer;

   [Export]
   private PackedScene _collectionLog;

   [ExportGroup("Equipment")]
   [Export]
   private PackedScene _equipmentButton;

   [Export]
   private PackedScene _equipmentInfo;

   [Export]
   private PackedScene _equipmentInfoButton;

   [Export]
   private PackedScene _equipmentInfoPopup;


   [ExportGroup("Palico")]
   [Export]
   private PackedScene _palicoEquipmentInfo;

   [Export]
   private PackedScene _palicoEquipmentInfoPopup;

   [Export]
   private PackedScene _palicoInfo;

   [Export]
   private PackedScene _palicoCraftButton;

   [Export]
   private PackedScene _palicoCraftOptionButton;

   [ExportSubgroup("Interfaces")]
   [Export]
   private PackedScene _palicoLoadoutInterface;

   [Export]
   private PackedScene _palicoCraftOptionInterface;

   [ExportGroup("Interfaces")]
   [Export]
   private PackedScene _equipmentSelectionInterface;

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

   public EquipmentSelectionButton GetEquipmentInfoButton(Equipment equipment, int index)
   {
      EquipmentSelectionButton equipmentSelectionButton = _equipmentInfoButton.Instantiate<EquipmentSelectionButton>();
      equipmentSelectionButton.SetEquipment(equipment, index);
      return equipmentSelectionButton;
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

   public PalicoLoadoutInterface GetPalicoLoadoutInterface(Palico palico)
   {
      PalicoLoadoutInterface palicoLoadoutInterface = _palicoLoadoutInterface.Instantiate<PalicoLoadoutInterface>();
      palicoLoadoutInterface.SetPalico(palico);
      return palicoLoadoutInterface;
   }

   public MonsterHealthBar GetMonsterHealthBar(Monster monster)
   {
      MonsterHealthBar monsterHealthBar = _monsterHealthBar.Instantiate<MonsterHealthBar>();
      monsterHealthBar.SetMonster(monster);

      return monsterHealthBar;
   }

   public PalicoEquipmentInfo GetPalicoEquipmentInfo(Palico palico, PalicoEquipmentType equipmentType)
   {
      PalicoEquipmentInfo palicoEquipmentInfo = _palicoEquipmentInfo.Instantiate<PalicoEquipmentInfo>();
      palicoEquipmentInfo.SetPalico(palico, equipmentType);
      return palicoEquipmentInfo;
   }

   public PalicoEquipmentInfoPopup GetPalicoEquipmentInfoPopup(Palico palico, PalicoEquipment equipment, int index)
   {
      PalicoEquipmentInfoPopup palicoEquipmentInfoPopup = _palicoEquipmentInfoPopup.Instantiate<PalicoEquipmentInfoPopup>();
      palicoEquipmentInfoPopup.SetEquipment(palico, equipment, index);
      return palicoEquipmentInfoPopup;
   }

   public PalicoCraftButton GetPalicoCraftButton(PalicoEquipment equipment)
   {
      PalicoCraftButton palicoCraftButton = _palicoCraftButton.Instantiate<PalicoCraftButton>();
      palicoCraftButton.SetEquipment(equipment);
      return palicoCraftButton;
   }

   public PalicoCraftOptionButton GetPalicoCraftOptionButton(PalicoEquipment equipment)
   {
      PalicoCraftOptionButton palicoCraftOptionButton = _palicoCraftOptionButton.Instantiate<PalicoCraftOptionButton>();
      palicoCraftOptionButton.SetEquipment(equipment);
      return palicoCraftOptionButton;
   }

   public PalicoInfo GetPalicoInfo(Palico palico)
   {
      PalicoInfo palicoInfo = _palicoInfo.Instantiate<PalicoInfo>();
      palicoInfo.SetPalico(palico);
      return palicoInfo;
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

   public ChargeBarTimer GetChargeBarTimer(float timeThreshold)
   {
      ChargeBarTimer chargeBarTimer = _chargeBarTimer.Instantiate<ChargeBarTimer>();
      chargeBarTimer.SetTimeThreshold(timeThreshold);
      return chargeBarTimer;
   }

   // * Interface scenes
   public RecipeInterface GetRecipeInterface(Equipment equipment, bool isCrafting, int index)
   {
      RecipeInterface recipeInterface = _recipeInterface.Instantiate<RecipeInterface>();
      recipeInterface.SetMaterials(equipment, isCrafting, index);
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

   public EquipmentSelectionInterface GetEquipmentSelectionInterface(Palico palico, PalicoEquipmentType equipmentType)
   {
      EquipmentSelectionInterface equipmentSelectionInterface = _equipmentSelectionInterface.Instantiate<EquipmentSelectionInterface>();
      equipmentSelectionInterface.SetEquipment(palico, equipmentType);
      return equipmentSelectionInterface;
   }

   public ItemBoxInterface GetItemBoxInterface()
   {
      return _itemBoxInterface.Instantiate<ItemBoxInterface>();
   }

   public EquipmentInfoPopup GetEquipmentInfoPopup(Equipment equipment)
   {
      EquipmentInfoPopup equipmentInfoPopup = _equipmentInfoPopup.Instantiate<EquipmentInfoPopup>();
      equipmentInfoPopup.SetEquipment(equipment);
      return equipmentInfoPopup;
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

   public LoadoutInterface GetLoadoutInterface()
   {
      return _loadoutInterface.Instantiate<LoadoutInterface>();
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

   public PalicoCraftOptionInterface GetPalicoCraftOptionInterface(PalicoEquipment equipment, int index)
   {
      PalicoCraftOptionInterface palicoCraftOptionInterface = _palicoCraftOptionInterface.Instantiate<PalicoCraftOptionInterface>();
      palicoCraftOptionInterface.Equipment = equipment;
      palicoCraftOptionInterface.Index = index;
      return palicoCraftOptionInterface;
   }
}