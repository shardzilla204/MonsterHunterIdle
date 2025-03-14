using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class PalicoDetails : NinePatchRect
{
	[Export]
	private LineEdit _palicoNameEdit;

	[Export]
	private PalicoModeButton _palicoModeButton;

	[Export]
	private CustomButton _palicoLoadoutButton;

	[Export]
	private Container _statsContainer;

	public PalicoData Palico;

	public override void _Ready()
	{
		_palicoNameEdit.TextSubmitted += (string text) => 
		{
			_palicoNameEdit.ReleaseFocus();
			if (text == "")
			{
				_palicoNameEdit.Text = Palico.Name;
				return;
			}
			Palico.Name = text;
			MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.OpenedPalicoLoadout, Palico);
		};
		_palicoNameEdit.Text = Palico.Name;
		_palicoModeButton.SetMode(Palico);
		_palicoLoadoutButton.Pressed += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.OpenedPalicoLoadout, Palico);
		FillStats();
	}

	private void FillStats()
	{
		ClearStats();
		List<StatType> statTypes = new List<StatType>()
		{
			StatType.Attack,
			StatType.Defense,
			StatType.Affinity
		};

		List<int> statValues = new List<int>()
		{
			Palico.Attack,
			Palico.Defense,
			Palico.Affinity
		};

		for (int i = 0; i < statTypes.Count; i++)
		{
			SetStat(statTypes[i], statValues[i]);
		}
	}

	private void ClearStats()
	{
		foreach (Node child in _statsContainer.GetChildren())
		{
			_statsContainer.RemoveChild(child);
			child.QueueFree();
		}
	}

	private void SetStat(StatType statType, int value)
	{
		StatDetail statDetail = MonsterHunterIdle.PackedScenes.GetStatDetail();
		statDetail.FillPalicoStat(statType, $"{value}");
		_statsContainer.AddChild(statDetail);
	}
}
