using TMPro;
using UnityEngine;

/// <summary>
/// Simple score manager to hold currentScore and persist BestScore using PlayerPrefs.
/// Attach one GameObject in the initial scene and set DontDestroyOnLoad.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int currentScore = 0;

    private const string BestScoreKey = "BestScore";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetBestScore()
    {
        return PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    /// <summary>
    /// If score > stored best, update and save. Returns true if saved.
    /// </summary>
    public bool TrySetNewBest(int score)
    {
        int best = GetBestScore();
        if (score > best)
        {
            PlayerPrefs.SetInt(BestScoreKey, score);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Add amount to the currentScore (called when player earns gold from sales).
    /// Also checks and updates best score immediately.
    /// </summary>
    public void AddToCurrentScore(int amount)
    {
        if (amount == 0) return;
        long sum = (long)currentScore + amount;
        if (sum > int.MaxValue) currentScore = int.MaxValue;
        else if (sum < int.MinValue) currentScore = int.MinValue;
        else currentScore = (int)sum;

        // Update persistent best if surpassed
        TrySetNewBest(currentScore);

        // Update LatestScore for MainMenu display
        PlayerPrefs.SetInt("LatestScore", currentScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Reset current score (call at start of run if needed).
    /// </summary>
    public void ResetCurrentScore()
    {
        currentScore = 0;
        PlayerPrefs.SetInt("LatestScore", 0);
        PlayerPrefs.Save();
    }
}
