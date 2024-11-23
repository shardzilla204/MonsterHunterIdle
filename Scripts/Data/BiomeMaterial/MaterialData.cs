using Godot;

namespace MonsterHunterIdle;

public partial class MaterialData : Resource
{
	[Export]
	private string _name;
	
	[Export(PropertyHint.MultilineText)] 
	private string _description;

	[Export]
	private int _quantity;

	[Export] 
	private Texture2D _icon;

	[Export]
	private RarityLevel _rarity;

	public string Name => _name;
	public string Description => _description;
	public int Quantity;
	public Texture2D Icon => _icon;
	public RarityLevel Rarity => _rarity;

	public const int MaxQuantity = 999;

	public void SetValues()
	{
		Quantity = _quantity;
	}
}
