using Godot;

namespace MonsterHunterIdle;

public partial class StarContainer : Container
{
	[Export]
	private Texture2D _yellowStar;

	[Export]
	private Texture2D _purpleStar;

	public void Fill()
	{
		Empty();
		for (int i = 0; i < MonsterManager.Instance.GetLevelValue(); i++)
		{
			AddChild(GetStarTexture());
		}
	}

	private TextureRect GetStarTexture()
	{
		if (MonsterManager.Instance.GetLevelValue() <= 5)
		{
			TextureRect textureRect = new TextureRect()
			{
				Texture = _yellowStar,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
			};
			return textureRect;
		}
		else
		{
			TextureRect textureRect = new TextureRect()
			{
				Texture = _purpleStar,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
			};
			return textureRect;
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
}
