using Godot;

namespace MonsterHunterIdle;

public partial class StarContainer : Container
{
	[Export]
	private Texture2D _yellowStar;

	[Export]
	private Texture2D _purpleStar;

	public void FillContainer()
	{
		EmptyContainer();
		Monster monster = MonsterHunterIdle.MonsterManager.Encounter.Monster;
		for (int i = 0; i < monster.Level; i++)
		{
			AddChild(GetStarTexture());
		}
	}

	public void EmptyContainer()
	{
		foreach(Node star in GetChildren())
		{
			RemoveChild(star);
			star.QueueFree();
		}
	}

	private TextureRect GetStarTexture()
	{
		Monster monster = MonsterHunterIdle.MonsterManager.Encounter.Monster;

		return monster.Level <= 5 ? GetStarTextureRect(_yellowStar) : GetStarTextureRect(_purpleStar);
	}

	private TextureRect GetStarTextureRect(Texture2D starTexture)
	{
		return new TextureRect()
		{
			Texture = starTexture,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
		};
	}
}
