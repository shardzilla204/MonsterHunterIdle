using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public enum MonsterSpecies
{
    GreatJagras,
    KuluYaKu,
    PukeiPukei
}

public enum MonsterLevel
{
	One = 2500,
	Two = 2000,
	Three = 1500,
	Four = 1000,
	Five = 500,
	Six = 250,
	Seven = 200,
	Eight = 150,
	Nine = 100,
	Ten = 50
}

[GlobalClass]
public partial class MonsterData : Resource
{   
    [Export]
    private string _name;


    [Export(PropertyHint.MultilineText)]
    private string _description;

    [Export]
    private MonsterSpecies _species;

    [Export]
    private Texture2D _icon;

    [Export]
    private Texture2D _render;

    [Export]
    private int _health;

    [Export]
    private MonsterLevel _level = MonsterLevel.One;

    [Export]
    private Array<BiomeType> _locale = new Array<BiomeType>();

    [Export]
    private Array<ElementType> _elements = new Array<ElementType>();

    [Export]
    private Array<AbnormalStatusType> _abnormalStatuses = new Array<AbnormalStatusType>();

    [Export]
    private Array<ElementType> _elementalWeaknesses = new Array<ElementType>();

    [Export]
    private Array<AbnormalStatusType> _abnormalStatusWeaknesses = new Array<AbnormalStatusType>();

    [Export]
    private Array<MonsterMaterialData> _materials = new Array<MonsterMaterialData>();

    public string Name => _name;
    public string Description => _description;
    public MonsterSpecies Species => _species;
    public Texture2D Icon => _icon;
    public Texture2D Render => _render;
    public int Health;
    public MonsterLevel Level;
    public List<BiomeType> Locale => _locale.ToList();
    public List<ElementType> Elements => _elements.ToList();
    public List<AbnormalStatusType> AbnormalStatuses => _abnormalStatuses.ToList();
    public List<ElementType> ElementalWeaknesses => _elementalWeaknesses.ToList();
    public List<AbnormalStatusType> AbnormalStatusWeaknesses => _abnormalStatusWeaknesses.ToList();
    public List<MonsterMaterialData> Materials => _materials.ToList();

    public void SetValues()
    {
        Health = _health;
        Level = _level;
    }
}
