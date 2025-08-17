using Godot;

namespace MonsterHunterIdle;

public partial class Popups : Control
{
    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.Popup += Popup;
    }

    private void Popup(Control node)
    {
        node.ZIndex = 1;
        node.Position = GetGlobalMousePosition();
        AddChild(node);
    }
}
