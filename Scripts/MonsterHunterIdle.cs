using Godot;

namespace MonsterHunterIdle;

public enum AttackType
{
	Severing, 
	Blunt
}

public enum ElementType
{
	Fire,
	Water,
	Thunder,
	Ice,
	Dragon
}

public enum AbnormalStatusType
{
	Poison,
	Paralysis,
	Sleep,
	Stun,
	BlastBlight,
	BubbleBlight,
	HellfireBlight,
}

public enum StatType
{
	Attack,
	Defense,
	Affinity,
	Water,
	Fire,
	Thunder,
	Ice,
	Dragon,
	Health
}

public partial class MonsterHunterIdle : Node
{
	[Export]
	private bool _areConsoleMessagesEnabled;

	public static HunterManager HunterManager = new HunterManager();
	public static PalicoManager PalicoManager = new PalicoManager();
	public static MaterialManager MaterialManager = new MaterialManager();
	public static ItemBox ItemBox = new ItemBox();
	public static BiomeManager BiomeManager = new BiomeManager();
	public static MonsterManager MonsterManager = new MonsterManager();
	
	public static Signals Signals = new Signals();
	public static PackedScenes PackedScenes = new PackedScenes();

	public static bool AreConsoleMessagesEnabled;

	public override void _Ready()
	{
		AreConsoleMessagesEnabled = _areConsoleMessagesEnabled;
	}s

   public static Texture2D GetIcon(string iconName)
	{
		string fileDirectory = "res://Assets/Images/Icon/";
		string fileName = $"{iconName}Icon";
		string fileExtension = ".png";

		string filePath = $"{fileDirectory}{fileName}{fileExtension}";

		return ResourceLoader.Load<Texture2D>(filePath);
	}

	public static Texture2D GetMonsterIcon(string monsterName)
	{
		string fileDirectory = "res://Assets/Images/Monster/Icon/";
		string fileName = $"{monsterName}Icon";
		string fileExtension = ".png";

		string filePath = $"{fileDirectory}{fileName}{fileExtension}";

		return ResourceLoader.Load<Texture2D>(filePath);
	}

	public static Texture2D GetMonsterRender(string monsterName)
	{
		string fileDirectory = "res://Assets/Images/Monster/Render/";
		string fileName = $"{monsterName}Render";
		string fileExtension = ".png";

		string filePath = $"{fileDirectory}{fileName}{fileExtension}";

		return ResourceLoader.Load<Texture2D>(filePath);
	}
}
