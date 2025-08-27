using Godot;

namespace MonsterHunterIdle;

public partial class HunterInterface : NinePatchRect
{
	[Export]
	private Label _hunterRankLabel;

	[Export]
	private HunterProgress _hunterProgress;

	[Export]
	private Label _zennyLabel;

	[Export]
	private Container _monsterSlayedContainer;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.HunterLeveledUp -= OnHunterLeveledUp;
		MonsterHunterIdle.Signals.MonsterSlayed -= OnMonsterSlayed;
	}

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.HunterLeveledUp += OnHunterLeveledUp;
		MonsterHunterIdle.Signals.MonsterSlayed += OnMonsterSlayed;
	}

	public override void _Ready()
	{
		// Set initially
		OnHunterLeveledUp();
		SetMonstersSlayed();
	}

	private void OnHunterLeveledUp()
	{
		_hunterRankLabel.Text = $"HR {MonsterHunterIdle.HunterManager.Hunter.Rank}";
		_hunterProgress.Update();
		_zennyLabel.Text = $"{MonsterHunterIdle.HunterManager.Hunter.Zenny}z";
	}

	private void SetMonstersSlayed()
	{
		// Clear out current node
		foreach (Node monsterSlayedNode in _monsterSlayedContainer.GetChildren())
		{
			monsterSlayedNode.QueueFree();
		}

		HunterManager hunterManager = MonsterHunterIdle.HunterManager;
		foreach (string monsterName in hunterManager.MonstersSlayed.Keys)
		{
			int slayCount = hunterManager.MonstersSlayed[monsterName];
			HBoxContainer monsterSlayedNode = GetMonsterSlayedNode(monsterName, slayCount);
			_monsterSlayedContainer.AddChild(monsterSlayedNode);
		}
	}

	private HBoxContainer GetMonsterSlayedNode(string monsterName, int slayCount)
	{
		int separation = 10;
		HBoxContainer monsterSlayedNode = new HBoxContainer()
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			Alignment = BoxContainer.AlignmentMode.Center
		};
		monsterSlayedNode.AddThemeConstantOverride("separation", separation);

		int textureSize = 50;
		Texture2D monsterTexture = MonsterHunterIdle.GetMonsterIcon(monsterName);
		TextureRect monsterIcon = new TextureRect()
		{
			Texture = monsterTexture,
			ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			CustomMinimumSize = new Vector2(textureSize, textureSize)
		};

		int fontSize = 20;
		Label monsterSlayedCount = new Label()
		{
			Text = $"{slayCount}"
		};
		monsterSlayedCount.AddThemeFontSizeOverride("font_size", fontSize);

		monsterSlayedNode.AddChild(monsterIcon);
		monsterSlayedNode.AddChild(monsterSlayedCount);

		return monsterSlayedNode;
	}

	private void OnMonsterSlayed(Monster monster)
	{
		SetMonstersSlayed();
	}
}
