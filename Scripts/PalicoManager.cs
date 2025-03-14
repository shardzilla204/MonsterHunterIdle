using Godot;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class PalicoManager : Node
{
	public PalicoManager()
	{
		GatheringInterval();
		HuntingInterval();
	}

	public List<Palico> Palicos = new List<Palico>();
	public int MaxPalicoCount = 10;

   private async void GatheringInterval()
	{
		float timeSeconds = 5f;
		while (true)
		{
			await ToSignal(GetTree().CreateTimer(timeSeconds), SceneTreeTimer.SignalName.Timeout);

			List<Palico> gatheringPalicos = FindGatheringPalicos();

			if (gatheringPalicos.Count == 0) continue;
			
			foreach (Palico gatheringPalico in gatheringPalicos)
			{
				BiomeMaterial biomeMaterial = MonsterHunterIdle.BiomeManager.GetBiomeMaterial();

				string palicoGatheredMessage = $"{gatheringPalico.Name} Has Picked Up {biomeMaterial.Name}";
				PrintRich.Print(TextColor.Yellow, palicoGatheredMessage);

				MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoGathered, gatheringPalico);
			}
		}
	}

	private async void HuntingInterval()
	{
		float timeSeconds = 5f;
		while (true)
		{
			await ToSignal(GetTree().CreateTimer(timeSeconds), SceneTreeTimer.SignalName.Timeout);

			List<Palico> huntingPalicos = FindHuntingPalicos();

			if (huntingPalicos.Count == 0) continue;

			foreach (Palico huntingPalico in huntingPalicos)
			{
				MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoHunted, huntingPalico);
			}
		}
	}

	private List<Palico> FindGatheringPalicos()
	{
		return Palicos.FindAll(palico => palico.Mode == PalicoMode.Gather);
	}

	private List<Palico> FindHuntingPalicos()
	{
		return Palicos.FindAll(palico => palico.Mode == PalicoMode.Hunt);
	}
}
