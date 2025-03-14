using Godot;

namespace MonsterHunterIdle;

public partial class RecruitPalicoButton : CustomButton
{
	[Signal]
	public delegate void RecruitedEventHandler();

	[Export]
	private Label _priceLabel;

	[Export]
	private int _basePrice = 100;

	private int _currentPrice;

    public override void _Ready()
    {
		base._Ready();
		Pressed += Buy;
		CalculatePrice();
    }

	private void Buy()
	{
		if (GameManager.Instance.Player.Palicos.Count == GameManager.Instance.Player.MaxPalicoAmount) return;
		if (GameManager.Instance.Player.Zenny < _currentPrice) return;

		GameManager.Instance.Player.Zenny -= _currentPrice;
		EmitSignal(SignalName.Recruited);
		CalculatePrice();
	}

	private void CalculatePrice()
	{
		_currentPrice = GameManager.Instance.Player.Palicos.Count switch 
		{
			0 => _basePrice,
			_ => Mathf.RoundToInt(GameManager.Instance.Player.Palicos.Count * _basePrice * 1.35f)
		};
		_priceLabel.Text = $"{_currentPrice}";
	}
}
