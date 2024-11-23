using Godot;

namespace MonsterHunterIdle;

public enum GreatJagrasMaterial
{
    Scale,
    Hide,
    Claw,
    Mane,
    Primescale,
    FangedWyvernGem,
    WyvernGemShard
}

[GlobalClass]
public partial class GreatJagrasMaterialData : MonsterMaterialData
{
    [Export]
    private GreatJagrasMaterial _material;

    public GreatJagrasMaterial Material => _material;
}
