using Godot;

namespace MonsterHunterIdle;

public partial class SmithyDisplay : VBoxContainer
{
	[Export]
	private ButtonGroup _entityGroup;

	[Export]
	private ButtonGroup _equipmentGroup;

	[Export]
	private EquipmentDisplayContainer _equipmentDisplayContainer;

    public override void _Ready()
    {
        _entityGroup.ButtonChanged += _equipmentDisplayContainer.ButtonChanged;
		_equipmentGroup.ButtonChanged += _equipmentDisplayContainer.ButtonChanged;
    }
}
