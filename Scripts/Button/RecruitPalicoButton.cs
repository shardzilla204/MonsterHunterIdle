using Godot;

namespace MonsterHunterIdle;

public partial class RecruitPalicoButton : CustomButton
{
	[Signal]
	public delegate void PalicoRecruitedEventHandler();

	[Export]
	private Label _priceLabel;

	[Export]
	private int _basePrice = 100;

	private int _currentPrice;

    public override void _Ready()
    {
		base._Ready();
		Pressed += OnPressed;
		
		CalculatePrice();
    }

	// Recruit palico
	private void OnPressed()
	{
		if (MonsterHunterIdle.PalicoManager.Palicos.Count == MonsterHunterIdle.PalicoManager.MaxPalicoCount) return;
		if (MonsterHunterIdle.HunterManager.Hunter.Zenny < _currentPrice) return;

		MonsterHunterIdle.HunterManager.Hunter.Zenny -= _currentPrice;
		EmitSignal(SignalName.PalicoRecruited);
		CalculatePrice();
	}

	private void CalculatePrice()
	{
		_currentPrice = MonsterHunterIdle.PalicoManager.Palicos.Count switch 
		{
			0 => _basePrice,
			_ => Mathf.RoundToInt(MonsterHunterIdle.PalicoManager.Palicos.Count * _basePrice * 1.35f)
		};
		_priceLabel.Text = $"{_currentPrice}";
	}
}
