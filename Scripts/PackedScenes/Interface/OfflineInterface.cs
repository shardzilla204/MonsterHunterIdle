using System.Collections.Generic;
using System.Linq;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public partial class OfflineInterface : ColorRect
{
    [Export]
    private CustomButton _exitButton;

    [Export]
    private Label _timeDifference;

    [Export]
    private Label _palicosLabel;

    [Export]
    private Container _materialLogContainer;

    private int _seconds = 0;

    public override void _Ready()
    {
        _exitButton.Pressed += QueueFree;

        GetRewards();
    }

    public void SetTime(GC.Dictionary<string, int> timeDifference)
    {
        int hours = timeDifference["hour"];
        int minutes = timeDifference["minute"];
        _seconds = HoursToSeconds(hours) + MinutesToSeconds(minutes) + timeDifference["second"];

        _timeDifference.Text = $"{PrintRich.GetTimeString(timeDifference)}";
    }

    // 1 Hour = 3600 Seconds
    private int HoursToSeconds(int hours)
    {
        int hoursToSecondsConversionRate = 3600;
        return hours * hoursToSecondsConversionRate;
    }

    // 1 Minute = 60 Seconds
    private int MinutesToSeconds(int minutes)
    {
        int minutesToSecondsConversionRate = 60;
        return minutes * minutesToSecondsConversionRate;
    }

    private void GetRewards()
    {
        int palicoCount = PalicoManager.Palicos.Count;
        if (palicoCount == 0)
        {
            _palicosLabel.Text += " None";
            return;
        }

        List<Material> rewards = new List<Material>();
        int gatherThreshold = (int) (PalicoManager.ActionIntervalSeconds * MonsterHunterIdle.OfflineThresholdMult);
        int gatherCount = _seconds / gatherThreshold;

        // Get how many materials to get depending on how many palicos there are
        for (int i = 0; i < palicoCount; i++)
        {
            for (int j = 0; j < gatherCount; j++)
            {
                Material material = MonsterHunterIdle.GetRandomMaterial();
                rewards.Add(material);
            }
        }

        // Add material logs 
        IEnumerable<Material> distinctRewards = rewards.Distinct();
        foreach (Material distinctReward in distinctRewards)
        {
            int materialCount = rewards.FindAll(reward => reward.Name == distinctReward.Name).Count;
            MaterialLog materialLog = MonsterHunterIdle.PackedScenes.GetMaterialLog(distinctReward, materialCount);
            _materialLogContainer.AddChild(materialLog);
        }

        ItemBox.Materials.AddRange(rewards);
    }
}
