using Godot;

public partial class CustomButton : Button
{
	[Export(PropertyHint.Range, "0,1,0.05")]
	private float _alpha = 1f;

	public bool IsTogglable = false;

	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

		OnMouseExited();
	}

	private void SetColor(Color color)
	{
		Modulate = color;
	}

	private void OnMouseEntered()
	{
		if (IsTogglable) return;

		Color color = Colors.White;
		color.A = _alpha;
		color = color.Darkened(0.3f);
		SetColor(color);
	}

	private void OnMouseExited()
	{
		if (IsTogglable) return;

		Color color = Colors.White;
		color.A = _alpha;
		SetColor(color);
	}

	public void OnButtonToggled(bool value)
	{
		Color color = Colors.White;
		color.A = _alpha;
		if (value)
		{
			color = color.Darkened(0.25f);
		}
		SetColor(color);
	}
}
