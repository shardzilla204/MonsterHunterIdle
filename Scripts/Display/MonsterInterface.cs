using Godot;

namespace MonsterHunterIdle;

public partial class MonsterInterface : NinePatchRect
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

	private Monster _monster;

	public override void _Ready()
	{
		MonsterManager.Instance.Encounter.Encountered += SetEncounter;
		TreeExited += () => MonsterManager.Instance.Encounter.Encountered -= SetEncounter;

		BiomeManager.Instance.Updated += LeftMonster;
		TreeExited += () => BiomeManager.Instance.Updated -= LeftMonster;

		_monsterHealthBar.Depleted += SlayedMonster;
		_attackButton.Pressed += AttackedMonster;
		MonsterHunterIdle.Signals.MonsterLeft += TimeoutMonster;
		_exitButton.Pressed += EscapedMonster;

		ClearEncounter();
	}

	private void SetEncounter(MonsterData monster)
	{
		string textString = $"★{MonsterManager.Instance.GetStarCount(monster.Level)} {MonsterManager.Instance.Encounter.Monster.Name}";
		GD.PrintRich($"A {PrintRich.SetTextColor(textString, TextColor.Red)} has entered the locale");

		_monsterName.Text = monster.Name;
		_monsterIcon.Texture = monster.Icon;
		_monsterRender.Texture = monster.Render;
		_monsterHealthBar.Visible = true;
		_monsterTimer.Start();
		_monsterHealthBar.Fill();
		_starContainer.Fill();
		_exitButton.Visible = true;
	}

	private void ClearEncounter()
	{
		_monsterName.Text = "";
		_monsterIcon.Texture = null;
		_monsterRender.Texture = null;
		_monsterHealthBar.Visible = false;
		_monsterTimer.Stop();
		_starContainer.Empty();
		_exitButton.Visible = false;

		if (MonsterManager.Instance.Encounter.Monster is null) return;
		
		SetEncounter(MonsterManager.Instance.Encounter.Monster);
	}

	private void AttackedMonster()
	{
		if (MonsterManager.Instance.Encounter.Monster is null) return;

		MonsterManager.Instance.EmitSignal(MonsterManager.SignalName.Damaged, GameManager.Instance.Player.Attack);
	}

	private void SlayedMonster()
	{
		MonsterData monster = MonsterManager.Instance.Encounter.Monster;

		if (monster is null) return;

		string monsterMessage = PrintRich.GetMonsterMessage(monster);
		string monsterLevelString = $"★{MonsterManager.Instance.GetStarCount(monster.Level)}";
		string monsterNameString = $"{MonsterManager.Instance.Encounter.Monster.Name}";
		string monsterSlayedMessage = $"The {monsterLevelString} {monsterNameString} has been slayed";
		PrintRich.Print(TextColor.Orange, monsterSlayedMessage);

		MonsterManager.Instance.Encounter.GetEncounterRewards();
		MonsterManager.Instance.Encounter.Monster = null;
		ClearEncounter();
	}

	private void OnMonsterLeft()
	{
		MonsterData monster = MonsterManager.Instance.Encounter.Monster;
		string textString = $"★{MonsterManager.Instance.GetStarCount(monster.Level)} {MonsterManager.Instance.Encounter.Monster.Name}";
		GD.PrintRich($"The {PrintRich.SetTextColor(textString, TextColor.Red)} has left the locale");
		MonsterManager.Instance.Encounter.Monster = null;
		ClearEncounter();
	}

	private void LeftMonster()
	{
		MonsterData monster = MonsterManager.Instance.Encounter.Monster;

		if (monster is null) return;

		string textString = $"★{MonsterManager.Instance.GetStarCount(monster.Level)} {MonsterManager.Instance.Encounter.Monster.Name}";
		GD.PrintRich($"You left the {PrintRich.SetTextColor(textString, TextColor.Red)} encounter in the biome");
		MonsterManager.Instance.Encounter.Monster = null;
		ClearEncounter();
	}

	private void EscapedMonster()
	{
		MonsterData monster = MonsterManager.Instance.Encounter.Monster;
		string textString = $"★{MonsterManager.Instance.GetStarCount(monster.Level)} {MonsterManager.Instance.Encounter.Monster.Name}";
		GD.PrintRich($"You escaped the {PrintRich.SetTextColor(textString, TextColor.Red)} encounter");
		MonsterManager.Instance.Encounter.Monster = null;
		ClearEncounter();
	}
}
