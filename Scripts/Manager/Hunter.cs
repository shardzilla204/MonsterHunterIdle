using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class Hunter : Node
{
   public int Rank = 1;
	public const int MaxRank = 999;

	public int Points;
	public int MaxPoints = 100;

	public List<PalicoData> Palicos = new List<PalicoData>();
	public int MaxPalicoAmount = 10;
	
	public int Zenny;

	public int Attack = 25;
	public int Affinity = 0;
	public int Defense = 100;
}