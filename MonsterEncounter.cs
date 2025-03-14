using Godot;

namespace MonsterHunterIdle;

public partial class MonsterEncounter : Node
{
	[Export]
	private float _maxEncounterChance = 100f;

	private float _encounterChance = 0f;

	public int Time;
	public int Health;
	public Monster Monster;

	public override void _Ready()
	{
   	MonsterHunterIdle.Signals.PalicoGathered += (palico) => IncreaseEncounterChance(); 
   }

   public void IncreaseEncounterChance()
	{
		if (Monster is not null) return;

		_encounterChance++;

		if (!CheckEncounterChance()) return;

		SetMonsterEncounter();
	}

	private bool CheckEncounterChance()
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();
		float currentChance = _maxEncounterChance - _encounterChance;
		float randomChance = RNG.RandfRange(_encounterChance, _maxEncounterChance);

		return randomChance >= currentChance ? true : false;
	}

	private void SetMonsterEncounter()
	{
		_encounterChance = 0f;

		Monster monster = MonsterHunterIdle.MonsterManager.GetRandomMonster();
		Monster = monster;

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterEncountered, monster);
	}

	public void GetEncounterRewards()
	{
		int pointsAmount = GetHunterPointsReward(Monster);
		MonsterHunterIdle.HunterManager.AddHunterPoints(pointsAmount);

		int zennyAmount = GetZennyReward(Monster);
		MonsterHunterIdle.HunterManager.AddHunterZenny(zennyAmount);
	}

	private int GetHunterPointsReward(Monster monster) => monster.Level switch
	{
		1 => 10,
		2 => 20,
		3 => 40,
		4 => 60,
		5 => 80,
		6 => 100,
		7 => 110,
		8 => 120,
		9 => 130,
		10 => 150,
		_ => 10
	};

	private int GetZennyReward(Monster monster) => monster.Level switch
	{
		1 => 10,
		2 => 20,
		3 => 30,
		4 => 40,
		5 => 50,
		6 => 100,
		7 => 110,
		8 => 130,
		9 => 140,
		10 => 150,
		_ => 10,
	};
}
