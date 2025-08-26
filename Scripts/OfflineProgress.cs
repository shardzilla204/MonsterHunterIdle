using System.Reflection;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class OfflineProgress : Node
{
    private string _offlineFilePath = "user://offline.time";
    private string _dataKey = "Time";

    public Dictionary<string, int> TimeDifference = new Dictionary<string, int>();

    public override void _EnterTree()
    {
        MonsterHunterIdle.OfflineProgress = this;

        GetWindow().CloseRequested += SaveProgress;
    }

    public override void _Ready()
    {
        if (FileAccess.FileExists(_offlineFilePath))
        {
            LoadProgress();
        }
        else
        {
            SaveProgress();
        }
    }

    public void SaveProgress()
    {
        using FileAccess gameFile = FileAccess.Open(_offlineFilePath, FileAccess.ModeFlags.Write);
        string jsonString = Json.Stringify(GetData(), "\t");

        if (jsonString == "") return;

        gameFile.StoreLine(jsonString);

        string saveSuccessMessage = "Game File Successfully Saved";
        if (PrintRich.AreFilePathsVisible)
        {
            saveSuccessMessage += $" At {gameFile.GetPathAbsolute()}";
        }
        PrintRich.PrintLine(TextColor.Green, saveSuccessMessage);
    }

    public void LoadProgress()
    {
        using FileAccess gameFile = FileAccess.Open(_offlineFilePath, FileAccess.ModeFlags.Read);
        string jsonString = gameFile.GetAsText();

        if (gameFile.GetLength() == 0)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Offline File Is Empty";
            PrintRich.PrintError(className, message);

            return;
        }

        Json json = new Json();

        Error result = json.Parse(jsonString);

        if (result != Error.Ok) return;

        Dictionary<string, Variant> offlineData = new Dictionary<string, Variant>((Dictionary)json.Data);
        CalculateTimeDifference(offlineData);

        string loadSuccessMessage = "Game File Successfully Loaded";
        if (PrintRich.AreFilePathsVisible)
        {
            loadSuccessMessage += $" At {gameFile.GetPathAbsolute()}";
        }
        PrintRich.PrintLine(TextColor.Green, loadSuccessMessage);
    }

    private Dictionary<string, Variant> GetData()
    {
        return new Dictionary<string, Variant>()
        {
            { _dataKey, Time.GetDatetimeDictFromSystem() }
        };
    }

    // Keys:
    /*
        year
        month
        day
        weekday
        hour
        minute
        second
        dst
    */
    private void CalculateTimeDifference(Dictionary<string, Variant> offlineData)
    {
        TimeDifference = new Dictionary<string, int>()
        {
            { "hour", GetTimeDifference(offlineData, "hour") },
            { "minute", GetTimeDifference(offlineData, "minute") },
            { "second", GetTimeDifference(offlineData, "second") },
        };
        PrintRich.PrintTimeDifference(TimeDifference);
    }

    private int GetTimeDifference(Dictionary<string, Variant> offlineData, string keyName)
    {
        Dictionary currentTime = Time.GetDatetimeDictFromSystem();
        Dictionary previousTime = offlineData[_dataKey].As<Dictionary>();

        int current = currentTime[keyName].As<int>();
        int previous = previousTime[keyName].As<int>();

        int timeDifference = current - previous < 0 ? current : current - previous;
        return timeDifference;
    }
}