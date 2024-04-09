using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance {get; set;}

    private string highScoreKey = "BestWaveSavedValue";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else {
            Instance = this;
        }

        DontDestroyOnLoad(this); // preserves this when the game scene loads
    }

    public void SaveHighScore(int score)
    {
        PlayerPrefs.SetInt(highScoreKey, score);
    }

    public int LoadHighScore()
    {

        if (PlayerPrefs.HasKey(highScoreKey)) {
            return PlayerPrefs.GetInt(highScoreKey);
        } else {
            return 0;
        }
    }
}
