using Godot;

namespace MonsterHunterIdle;

public partial class ElementalResistance : Node
{
	public ElementalResistance(ElementType elementType, int value)
	{
		ElementType = elementType;
		Value = value;
	}

	public ElementType ElementType;
	public int Value;
}
