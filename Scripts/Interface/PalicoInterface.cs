using Godot;

namespace MonsterHunterIdle;

public partial class PalicoInterface : Container
{
	[Export]
	private RecruitPalicoButton _recruitPalicoButton;

	[Export]
	private Label _palicoAmount;

	[Export]
	private Container _palicoContainer;

	private PalicoLoadoutInterface _palicoLoadoutInterface;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.ChangePalicoEquipmentButtonPressed -= OnChangePalicoEquipmentButtonPressed;
		MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.ChangePalicoEquipmentButtonPressed += OnChangePalicoEquipmentButtonPressed;
		MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
    }

	public override void _Ready()
	{
		foreach (Palico palico in PalicoManager.Palicos)
		{
			AddPalicoDetails(palico);
		}

		_recruitPalicoButton.PalicoRecruited += OnPalicoRecruited;

		UpdateText();
	}
	
	// * START - Signal Methods
    private void OnChangePalicoEquipmentButtonPressed(PalicoEquipmentType equipmentType)
    {
        EquipmentSelectionInterface equipmentSelectionInterface = MonsterHunterIdle.PackedScenes.GetEquipmentSelectionInterface(equipmentType);
        if (equipmentSelectionInterface == null) return;

        AddSibling(equipmentSelectionInterface);
    }

	private void OnPalicoRecruited()
	{
		// Console message
		string recruitedPalicoMessage = "Palico Has Been Added To The Team";
		PrintRich.PrintLine(TextColor.Yellow, recruitedPalicoMessage);

		Palico palico = new Palico();
		PalicoManager.Palicos.Add(palico);
		AddPalicoDetails(palico);
		OnLoadoutOpened(palico); // Show loadout
		CheckAmount();

		UpdateText();
	}

	private void OnInterfaceChanged(InterfaceType interfaceType)
	{
		QueueFree();
	}
    // * END - Signal Methods

	private void AddPalicoDetails(Palico palico)
	{
		PalicoDetails palicoDetails = MonsterHunterIdle.PackedScenes.GetPalicoDetails();
		palicoDetails.Palico = palico;
		palicoDetails.LoadoutOpened += OnLoadoutOpened;
		_palicoContainer.AddChild(palicoDetails);
	}

	private void OnLoadoutOpened(Palico palico)
	{
		if (IsInstanceValid(_palicoLoadoutInterface)) 
		{
			_palicoLoadoutInterface.QueueFree();
		} 

		_palicoLoadoutInterface = MonsterHunterIdle.PackedScenes.GetPalicoLoadoutInterface(palico);
		AddChild(_palicoLoadoutInterface); // For display orientation/placement as PalicoInterface is a container
	}

	private void UpdateText()
	{
		int palicoCount = PalicoManager.Palicos.Count;
		int maxPalicoCount = PalicoManager.MaxPalicoCount;

		_palicoAmount.Text = $"{palicoCount} / {maxPalicoCount}";
	}

	private void CheckAmount()
	{
		int palicoCount = PalicoManager.Palicos.Count;
		int maxPalicoCount = PalicoManager.MaxPalicoCount;

		_recruitPalicoButton.Disabled = palicoCount == maxPalicoCount ? true : false;
	}
}
