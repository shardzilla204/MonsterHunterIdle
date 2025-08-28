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
		if (PalicoManager.Palicos.Count == PalicoManager.MaxPalicoCount) return;
		if (Hunter.Zenny < _currentPrice) return;

		Hunter.Zenny -= _currentPrice;
		EmitSignal(SignalName.PalicoRecruited);
		CalculatePrice();
	}

	private void CalculatePrice()
	{
		float priceMult = 1.65f;
		_currentPrice = PalicoManager.Palicos.Count switch 
		{
			0 => _basePrice,
			_ => Mathf.RoundToInt(PalicoManager.Palicos.Count * _basePrice * priceMult)
		};
		_priceLabel.Text = $"{_currentPrice}";
	}
}
