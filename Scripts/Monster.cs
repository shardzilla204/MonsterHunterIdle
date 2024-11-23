using Godot;
using System;

namespace MonsterHunterIdle;

public partial class Monster : VBoxContainer
{   
    [Export] public TextureRect icon;
    [Export] public Area2D area;
    [Export] public TextureProgressBar healthBar;
    [Export] public PackedScene chargeBarScene;
    public TextureProgressBar chargeBar;

   	private bool isHovering = false;
	private bool isMouseDown = false;

    public override void _Ready()
    {
        area.MouseEntered += () => isHovering = true;
        area.MouseExited += () => isHovering = false;
        chargeBar = chargeBarScene.Instantiate<TextureProgressBar>();
        GameManager.Instance.AddChild(chargeBar);
    }

    public override void _Process(double delta)
    {
        UpdateChargeBar();
        CheckMonsterHealth();
    }

    public void UpdateChargeBar()
	{
        Vector2 mousePosition = GetViewport().GetMousePosition();
        chargeBar.Position = mousePosition - chargeBar.Size / 2;
        if (isHovering && Input.IsActionPressed("leftClick"))
        {
            chargeBar.Value++;
        }
        else
        {
            chargeBar.Value--;
        }
        CheckChargeBar();
	}

    public void CheckChargeBar()
	{	
		if (chargeBar.Value < chargeBar.MaxValue) return;
        AttackMonster();
		chargeBar.Value = 0;
	}

    public void UpdateMonsterHealth()
    {

    }

    public void CheckMonsterHealth()
    {
        // if (healthBar.Value <= healthBar.MinValue)
        // {
        //     for (int i = 0; i < 4; i++)
        //     {
        //         CollectionLogManager.Instance.AddLog(Game.biome.GetResource());
        //         Game.biome.UpdateCollectionLog();
        //     }
        //     QueueFree();
        // }
    }

    public void AttackMonster()
    {
        healthBar.Value -= 50;
    }
}
