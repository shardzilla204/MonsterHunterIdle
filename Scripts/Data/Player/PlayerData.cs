using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class PlayerData : Resource
{
	public int HunterRank = 1;
	public const int MaxHunterRank = 999;

	public int HunterProgress;
	public int MaxHunterProgress = 100;

	public List<PalicoData> Palicos = new List<PalicoData>();
	public int MaxPalicoAmount = 10;
	
	public int Zenny;

	public int Attack = 25;
	public int Affinity = 0;
	public int Defense = 100;

	public PlayerData(int hunterRank, int zenny)
	{
		HunterRank = hunterRank;
		Zenny = zenny;
	}
}
