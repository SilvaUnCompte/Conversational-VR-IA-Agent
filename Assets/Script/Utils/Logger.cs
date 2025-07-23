  using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Logger
{
    public static string Group;
    private static string analiticsFolder = "Output/analytics/";
    private static string surveyFolder = "Output/surveys/";

    private static List<ActionEntry> logs = new List<ActionEntry>();
    private static List<SurveyEntry> surveyEntries = new List<SurveyEntry>();
    private static float pointZero = 0f; // Start time for the first action

    public static void Initialize()
    {
        // Ensure the output directories exist
        System.IO.Directory.CreateDirectory(Application.dataPath + "/" + analiticsFolder + Group);
        System.IO.Directory.CreateDirectory(Application.dataPath + "/" + surveyFolder + Group);
        
        // Reset the pointZero to current time
        pointZero = Time.time;
        logs.Clear();
    }

    public static void AddLog(ActionType action, string metadata = "")
    {
        float currentTime = Time.time - pointZero;
        ActionEntry entry = new ActionEntry
        {
            time = currentTime,
            actionType = action,
            metadata = metadata.Replace(",", ".")
        };
        logs.Add(entry);
    }

    public static void SaveActionsToCSV()
    {
        StringBuilder sb = new StringBuilder();
        string path = Application.dataPath + "/" + analiticsFolder + Group + "/" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".csv";

        Debug.Log("Logger: Saving to " + path);

        sb.AppendLine("time,actionType,metadata");
        foreach (ActionEntry row in logs)
        {
            sb.AppendLine(row.ToString());
        }
        sb.AppendLine((Time.time - pointZero).ToString().Replace(",",".") + "," + ActionType.End);

        System.IO.File.WriteAllText(path, sb.ToString());
    }


    public static void AddSurveyEntry(string question, string answer)
    {
        SurveyEntry entry = new SurveyEntry
        {
            question = question,
            answer = answer
        };
        surveyEntries.Add(entry);
    }
    public static void SaveSurveyToCSV()
    {
        StringBuilder sb = new StringBuilder();
        string path = Application.dataPath + "/" + surveyFolder + Group + "/" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".csv";
        sb.AppendLine("question,answer");
        foreach (SurveyEntry row in surveyEntries)
        {
            sb.AppendLine(row.ToString());
        }
        System.IO.File.WriteAllText(path, sb.ToString());
    }
}

// ========== Enums and Structs ==========

public enum ActionType
{
    UserTalk,
    IATalk,
    Walk,
    Point,
    End
}

public struct ActionEntry
{
    public float time;
    public ActionType actionType;
    public string metadata;
    public override string ToString()
    {
        return $"{time.ToString().Replace(",", ".")},{actionType},{metadata}";
    }
}

public struct SurveyEntry
{
    public string question;
    public string answer;
    public override string ToString()
    {
        return $"{question},{answer}";
    }
}