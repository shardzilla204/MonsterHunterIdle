using Godot;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class PalicoManager : Node
{
	[Export(PropertyHint.Range, "1, 7.5, 0.5")]
	private float _intervalTimeSeconds = 5f;

	public List<Palico> Palicos = new List<Palico>();
	public int MaxPalicoCount = 10;

	public PalicoEquipmentManager Equipment;

	public override void _EnterTree()
	{
		MonsterHunterIdle.PalicoManager = this;
	}

	public override void _Ready()
	{
		ActionInterval();
	}

	private async void ActionInterval()
	{
		while (true)
		{
			await ToSignal(GetTree().CreateTimer(_intervalTimeSeconds), SceneTreeTimer.SignalName.Timeout);

			foreach (Palico palico in Palicos)
			{
				if (MonsterHunterIdle.MonsterManager.Encounter.IsInEncounter)
				{
					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoHunted, palico);
					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterDamaged, palico.Attack);
				}
				else
				{
					LocaleMaterial localeMaterial = MonsterHunterIdle.LocaleManager.GetLocaleMaterial();

					// Console message
					string palicoGatheredMessage = $"{palico.Name} Has Picked Up {localeMaterial.Name}";
					PrintRich.PrintLine(TextColor.Yellow, palicoGatheredMessage);

					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoGathered, palico);
					MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.LocaleMaterialAdded, localeMaterial);
				}
			}
		}
	}

	private int GetPalicoDamage(Palico palico)
	{
		return 1;
	}
}
