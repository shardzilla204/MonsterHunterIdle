using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class Locale : Node
{
	public Locale(LocaleType localeType)
	{
		Name = localeType.ToString();
		Type = localeType;
		LocaleIcon = MonsterHunterIdle.LocaleManager.GetLocaleIcon(localeType);
		Background = MonsterHunterIdle.LocaleManager.GetBackground(localeType);
		GatherIcon = MonsterHunterIdle.LocaleManager.GetGatherIcon(localeType);
	}

	public new string Name;
	public LocaleType Type;
	public Texture2D LocaleIcon;
	public Texture2D Background;
	public Texture2D GatherIcon;
	public List<LocaleMaterial> Materials = new List<LocaleMaterial>();
}