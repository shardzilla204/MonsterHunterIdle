using Godot;
using System;

namespace MonsterHunterIdle;

public partial class EquipButton : CustomButton
{
	public override void _Ready()
	{
		base._Ready();
		Pressed += GetEquipment;
	}

	public override void _Process(double delta)
	{
	}

	private void GetEquipment()
	{
		
	}
}
