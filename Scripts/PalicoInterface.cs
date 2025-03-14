using Godot;

namespace MonsterHunterIdle;

public partial class PalicoDisplay : Container
{
	[Export]
	private RecruitPalicoButton _recruitPalicoButton;

	[Export]
	private Label _palicoAmount;

	[Export]
	private PackedScene _palicoDetailsScene;

	[Export]
	private PackedScene _palicoLoadoutScene;

	private PalicoLoadout _palicoLoadout;

	[Export]
	private Container _palicoContainer;

    public override void _Ready()
    {
		foreach (PalicoData palico in GameManager.Instance.Player.Palicos)
		{
			AddPalicoDetails(palico);
		}

      _recruitPalicoButton.Recruited += () => 
		{
			RecruitPalico();
			UpdateText();
		};
		UpdateText();
    }

	private void RecruitPalico()
	{
		Palico palico = new Palico();
		string recruitedPalicoMessage = "Palico has been added to the team";
		PrintRich.Print(TextColor.Yellow, recruitedPalicoMessage);
		MonsterHunterIdle.PalicoManager.Palicos.Add(palico);
		AddPalicoDetails(palico);
		ShowLoadout(palico);
		CheckAmount();
	}

	private void AddPalicoDetails(Palico palico)
	{
		PalicoDetails palicoDetails = _palicoDetailsScene.Instantiate<PalicoDetails>();
		palicoDetails.Palico = palico;
		MonsterHunterIdle.Signals.OpenedPalicoLoadout += ShowLoadout;
		_palicoContainer.AddChild(palicoDetails);
	}

	private void ShowLoadout(Palico palico)
	{
		if (_palicoLoadout is not null) 
		{
			RemoveChild(_palicoLoadout);
			_palicoLoadout.QueueFree();
		} 
		
		_palicoLoadout = _palicoLoadoutScene.Instantiate<PalicoLoadout>();
		_palicoLoadout.Palico = palico;
		AddChild(_palicoLoadout);
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
