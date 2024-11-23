using Godot;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class PlayerData : Resource
{
	[Export]
	private int _hunterRank = 1;

	[Export]
	private int _hunterProgress = 0;

	[Export]
	private int _zenny = 0;

	public int HunterRank;
	public const int MaxHunterRank = 999;

	public int HunterProgress;
	public int MaxHunterProgress = 100;
	
	public int Zenny;

	public void SetValues()
	{
		HunterRank = _hunterRank;
		HunterProgress = _hunterProgress;
		Zenny = _zenny;
	}
}
