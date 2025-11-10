using TMPro;
using UnityEngine;

/// <summary>
/// Simple score manager to hold currentScore and persist BestScore using PlayerPrefs.
/// Attach one GameObject in the initial scene and set DontDestroyOnLoad.
/// </summary>
public class DisplayScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Update()
    {
        if (ScoreManager.Instance != null)
        {
            scoreText.text = ScoreManager.Instance.currentScore.ToString();
        }
    }
}
