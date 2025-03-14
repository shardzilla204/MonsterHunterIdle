using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class ButtonGroup : Container
{
	[Signal]
	public delegate void ButtonChangedEventHandler(int type);

	public enum Type 
	{
		Entity,
		Equipment
	}

	[Export]
	private Type _type;

	private List<CustomButton> _buttons = new List<CustomButton>();

	public override void _Ready()
	{
		Array<Node> children = GetChildren();

		foreach (Node child in children)
		{
			CustomButton button = (CustomButton) child;
			button.IsTogglable = true;
			_buttons.Add((CustomButton) child);
		}

		foreach (CustomButton button in _buttons)
		{
			button.Pressed += () => ToggleButton(button);
		}
		ToggleButton(_buttons[0]);
	}

	private void ToggleButton(CustomButton button)
	{
		button.OnButtonToggled(true);
		List<CustomButton> otherButtons = _buttons.FindAll(otherButton => otherButton != button);
		foreach (CustomButton otherButton in otherButtons)
		{
			otherButton.ButtonPressed = false;
			otherButton.OnButtonToggled(false);
		}

		EmitSignal(SignalName.ButtonChanged, (int) _type);
	}
}
