using Godot;

namespace MonsterHunterIdle;

public partial class StarContainer : Container
{
	[Export]
	private Texture2D _yellowStar;

	[Export]
	private Texture2D _purpleStar;

	public void Fill(Monster monster)
	{
		Empty();
		for (int i = 0; i < monster.Level; i++)
		{
			TextureRect starTexture = GetStarTexture(monster);
			AddChild(starTexture);
		}
	}

	public void Empty()
	{
		foreach(Node star in GetChildren())
		{
			RemoveChild(star);
			star.QueueFree();
		}
	}

	private TextureRect GetStarTexture(Monster monster)
	{
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
