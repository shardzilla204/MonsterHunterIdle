using Godot;

namespace MonsterHunterIdle;

public partial class StatDetail : Container
{
	[Export]
	private TextureRect _statIcon;

	[Export]
	private Label _statLabel;

	public void FillPalicoStat(StatType statType, string value)
	{
		_statIcon.Texture = MonsterHunterIdle.GetStatIcon(statType);
		_statLabel.Text = value;
	}
}
