using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class HighScoreManager : MonoBehaviour
{
    private string _state;
    public string State
    {
        get { return _state; }
        set { _state = value; }
    }

    private string _dataPath;
    
    private string _jsonScores;
    
    private List<ScoreInfo> _scores;

    void Awake()
    {
        _dataPath = Application.persistentDataPath + @"\HighScoreData\"; // @ = literal string
        Debug.Log(_dataPath);
        NewDirectory();

        _jsonScores = _dataPath + "High_Score_Data.json";
        NewTextFile();
    }
    
    void Start()
    {
        Initialize();
        //AddScore(new ScoreInfo("TST", 6000));
        
        //FilesystemInfo();
    }

    public void Initialize()
    {
        _state = "High Score Manager initialized.";
        Debug.Log(_state);
    }

    public void FilesystemInfo()
    {
        Debug.LogFormat("Path separator character: {0}", Path.
            PathSeparator);
        Debug.LogFormat("Directory separator character: {0}", Path.
            DirectorySeparatorChar);
        Debug.LogFormat("Current directory: {0}", Directory.
            GetCurrentDirectory());
        Debug.LogFormat("Temporary path: {0}", Path.GetTempPath());
    }

    public void NewDirectory()
    {
        if (Directory.Exists(_dataPath))
        {
            Debug.Log("Directory already exists");
            return;
        }
        
        Directory.CreateDirectory(_dataPath);
        Debug.Log("Directory created");
    }

    public void DeleteDirectory()
    {
        if (!Directory.Exists(_dataPath))
        {
            Debug.Log("Directory doesn't exist or has already been deleted");
            return;
        }
        
        Directory.Delete(_dataPath, true);
        Debug.Log("Directory deleted");
    }

    public void NewTextFile()
    {
        if (File.Exists(_jsonScores))
        {
            Debug.Log("File already exists");
            return;
        }
        
        File.WriteAllText(_jsonScores, "{}");
        Debug.Log("File created");
    }

    public void UpdateTextFile(string textToAdd)
    {
        if (!File.Exists(_jsonScores))
        {
            Debug.Log("File doesn't exist");
            return;
        }
        
        File.AppendAllText(_jsonScores, textToAdd);
        Debug.Log("File updated");
    }

    public void AddScore(ScoreInfo scoreToAdd)
    {
        // if score file exists, deserialize it, sort scores from highest to lowest, then re-serialize it
        if (File.Exists(_jsonScores))
        {
            using (StreamReader stream = new StreamReader(_jsonScores))
            {
                var jsonString = stream.ReadToEnd();
                
                var scoreData = JsonUtility.FromJson<HighScore>(jsonString);
                scoreData.ListOfScores.Add(scoreToAdd);
                scoreData.ListOfScores.Sort();

                // foreach (var score in scoreData.ListOfScores)
                // {
                //     Debug.LogFormat("{0} - {1}", score.PlayerName, score.Score);
                // }

                _scores = scoreData.ListOfScores;
            }
            
            // re-serializing and updating file
            HighScore highScores = new HighScore();
            highScores.ListOfScores = _scores;

            using (StreamWriter stream = File.CreateText(_jsonScores))
            {
                string jsonString = JsonUtility.ToJson(highScores, true);
                stream.WriteLine(jsonString);
            }
        }
    }

    public HighScore GetHighScores()
    {
        using (StreamReader stream = new StreamReader(_jsonScores))
        {
            var jsonString = stream.ReadToEnd();
            var scoreData = JsonUtility.FromJson<HighScore>(jsonString);

            return scoreData;
        }
    }
}
