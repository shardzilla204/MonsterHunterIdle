using Godot;
using System;

public partial class CraftingDisplay : NinePatchRect
{
	[Export]
	private CustomButton _exitButton;

	public override void _Ready()
	{
		_exitButton.Pressed += QueueFree;
	}
}
