using Godot;

namespace MonsterHunterIdle;

public partial class MonsterDisplay : NinePatchRect
{
	[Export]
	private Label _monsterName;

	[Export]
	private TextureRect _monsterIcon;

	[Export]
	private TextureRect _monsterRender;

	[Export]
	private MonsterHealthBar _monsterHealthBar;

	[Export]
	private StarContainer _starContainer;

	[Export]
	private Button _attackButton;

	[Export]
	private CustomButton _exitButton;

	[Export]
	private MonsterTimer _monsterTimer;

	private MonsterData _monster;

	private bool _isPresent = false;

	public override void _Ready()
	{
		MonsterManager.Instance.Encountered += SetEncounter;
		TreeExited += () => MonsterManager.Instance.Encountered -= SetEncounter;

		BiomeManager.Instance.Updated += () => 
		{
			if (_monster is null) return;

			GD.PrintRich($"You left the [color=RED]★{MonsterManager.Instance.GetLevelValue()} {_monster.Name}[/color] encounter in the biome");
			ClearEncounter();
		};
		TreeExited += () => BiomeManager.Instance.Updated -= ClearEncounter;

		_monsterHealthBar.Depleted += () => 
		{
			GD.PrintRich($"The [color=RED]★{MonsterManager.Instance.GetLevelValue()} {_monster.Name}[/color] has been slayed");
			MonsterManager.Instance.GetRewards();
			ClearEncounter();
		};
		_attackButton.Pressed += () => 
		{
			if (!_isPresent) return;

			_monsterHealthBar.DamageMonster(100);
		};
		_monsterTimer.Finished += () => 
		{
			GD.PrintRich($"The [color=RED]★{MonsterManager.Instance.GetLevelValue()} {_monster.Name}[/color] has left the locale");
			ClearEncounter();
		};
		_exitButton.Pressed += () =>
		{
			GD.PrintRich($"You escaped the [color=RED]★{MonsterManager.Instance.GetLevelValue()} {_monster.Name}[/color] encounter");
			ClearEncounter();
		};

		ClearEncounter();
	}

	private void SetEncounter(MonsterData monster)
	{
		if (_monster is not null) return;

		GD.PrintRich($"A [color=RED]★{MonsterManager.Instance.GetLevelValue()} {monster.Name}[/color] has entered the locale");

		_monster = monster;
		_monsterName.Text = monster.Name;
		_monsterIcon.Texture = monster.Icon;
		_monsterRender.Texture = monster.Render;
		_monsterHealthBar.Visible = true;
		_monsterTimer.Start();
		_monsterHealthBar.Fill();
		_starContainer.Fill();
		_exitButton.Visible = true;
		_isPresent = true;
	}

	private void ClearEncounter()
	{
		_monster = null;
		_monsterName.Text = "";
		_monsterIcon.Texture = null;
		_monsterRender.Texture = null;
		_monsterHealthBar.Visible = false;
		_monsterTimer.Stop();
		_starContainer.Empty();
		_exitButton.Visible = false;
		_isPresent = false;
	}
}
