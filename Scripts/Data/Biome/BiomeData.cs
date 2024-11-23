using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class BiomeData : Resource
{
    [Export] 
	public string Name;

	[Export(PropertyHint.MultilineText)] 
	public string Description;

	[Export]
	public BiomeType Type;

	[Export] 
	public Texture2D Icon;

	[Export]
	public Texture2D Background;

	[Export]
	public Texture2D GatherIcon;

	[Export]
	public Array<MonsterData> Monsters = new Array<MonsterData>();
}
