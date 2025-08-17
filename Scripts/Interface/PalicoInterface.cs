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

	// private PalicoLoadout _palicoLoadout;

    public override void _Ready()
    {
		foreach (Palico palico in MonsterHunterIdle.PalicoManager.Palicos)
		{
			AddPalicoDetails(palico);
		}

      	_recruitPalicoButton.PalicoRecruited += OnPalicoRecruited;

		UpdateText();
    }

	private void OnPalicoRecruited()
	{
		// Console message
		string recruitedPalicoMessage = "Palico Has Been Added To The Team";
		PrintRich.PrintLine(TextColor.Yellow, recruitedPalicoMessage);

		Palico palico = new Palico();
		MonsterHunterIdle.PalicoManager.Palicos.Add(palico);
		AddPalicoDetails(palico);
		OnLoadoutOpened(palico); // Show loadout
		CheckAmount();
		
		UpdateText();
	}

	private void AddPalicoDetails(Palico palico)
	{
		PalicoDetails palicoDetails = MonsterHunterIdle.PackedScenes.GetPalicoDetails();
		palicoDetails.Palico = palico;
		palicoDetails.LoadoutOpened += OnLoadoutOpened;
		_palicoContainer.AddChild(palicoDetails);
	}

	private void OnLoadoutOpened(Palico palico)
	{
		// if (_palicoLoadout is not null) 
		// {
		// 	RemoveChild(_palicoLoadout);
		// 	_palicoLoadout.QueueFree();
		// } 
		
		// _palicoLoadout = MonsterHunterIdle.PackedScenes.GetPalicoLoadout();
		// _palicoLoadout.Palico = palico;
		// AddChild(_palicoLoadout);
	}

	private void UpdateText()
	{
		int palicoCount = MonsterHunterIdle.PalicoManager.Palicos.Count;
		int maxPalicoCount = MonsterHunterIdle.PalicoManager.MaxPalicoCount;

		_palicoAmount.Text = $"{palicoCount} / {maxPalicoCount}";
	}

	private void CheckAmount()
	{
		int palicoCount = MonsterHunterIdle.PalicoManager.Palicos.Count;
		int maxPalicoCount = MonsterHunterIdle.PalicoManager.MaxPalicoCount;

		_recruitPalicoButton.Disabled = palicoCount == maxPalicoCount ? true : false;
	}
}
