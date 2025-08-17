using Godot;

public partial class CustomButton : Button
{
	[Export(PropertyHint.Range, "0,1,0.05")]
	private float _alpha = 1f;

	private float _darkValue = 0.3f;

	private NinePatchRect _buttonTexture;
	private Color _originalColor;

    public override void _EnterTree()
	{
		_buttonTexture = GetChildOrNull<NinePatchRect>(0);
		_originalColor = _buttonTexture.SelfModulate;
	}

	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		Toggled += OnToggled;

		OnMouseExited();
	}

	public void SetColor(Color color)
	{
		_buttonTexture.SelfModulate = color;
	}

	private void OnMouseEntered()
	{
		if (ToggleMode) return;

		Color color = _originalColor;
		color.A = _alpha;
		color = color.Darkened(_darkValue);
		SetColor(color);
	}

	private void OnMouseExited()
	{
		if (ToggleMode) return;

		Color color = _originalColor;
		color.A = _alpha;
		SetColor(color);
	}

	public void OnToggled(bool isToggled)
	{
		Color color = _originalColor;
		color.A = _alpha;
		if (isToggled)
		{
			color = color.Darkened(_darkValue);
		}
		SetColor(color);
	}
}
