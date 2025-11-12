using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <-- THÊM DÒNG NÀY
public class MainMenuManager : MonoBehaviour
{
    // Gán tên Scene game của bạn vào đây trong Inspector
    public static bool IsLoadingGame = false;
    public string gameSceneName = "MainScene"; // Thay "Game" bằng tên Scene game của bạn
    public GameObject guidePanel;
    public GameObject bestScorePanel; // Panel điểm cao
    public TextMeshProUGUI scoreText; // Text để hiện Best Score
    public TextMeshProUGUI latestScoreText; // Text để hiện Latest Score (vừa chơi xong)
    public GameObject settingsPanel;
    public SettingsManager settingsManager;

    void Start()
    {
        // If a previous scene set this flag, open the Best Score panel automatically
        if (PlayerPrefs.GetInt("ShowBestOnMenu", 0) == 1)
        {
            // Clear the flag
            PlayerPrefs.SetInt("ShowBestOnMenu", 0);
            PlayerPrefs.Save();
            OpenBestScore();
        }
    }


    public void StartGame()
    {
        IsLoadingGame = false; // Bắt đầu game mới
        SceneManager.LoadScene(gameSceneName);
    }

    // Hàm này cho nút Continue
    public void ContinueGame()
    {
        IsLoadingGame = true; // Tiếp tục game đã lưu
        SceneManager.LoadScene(gameSceneName);
    }

    // Hàm này cho nút Settings
    public void OpenSettings()
    {
        // Sau này bạn có thể tạo một panel cài đặt và bật nó ở đây
        Debug.Log("Mở màn hình cài đặt!");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        if (settingsManager != null)
        {
            settingsManager.OnSettingsOpen(); 
        }
    }

    // Hàm này cho nút Guide
    public void OpenGuide()
    {
        guidePanel.SetActive(true);
    }

    public void CloseGuide()
    {
        guidePanel.SetActive(false);
    }

    public void ShowBestScore()
    {
        Debug.Log("Hiển thị điểm cao!");
    }
    public void OpenBestScore()
    {
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        scoreText.text = "Best Score: " + bestScore.ToString();

        int latest = PlayerPrefs.GetInt("LatestScore", 0);
        if (latestScoreText != null)
        {
            latestScoreText.text = "Latest Score: " + latest.ToString();
        }

        bestScorePanel.SetActive(true);
    }

    public void CloseBestScore()
    {
        bestScorePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Thoát Game!");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void CloseSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}