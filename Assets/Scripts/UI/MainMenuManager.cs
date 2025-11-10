using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <-- THÊM DÒNG NÀY
public class MainMenuManager : MonoBehaviour
{
    // Gán tên Scene game của bạn vào đây trong Inspector
    public string gameSceneName = "MainScene"; // Thay "Game" bằng tên Scene game của bạn
    public GameObject guidePanel;
    public GameObject bestScorePanel; // Panel điểm cao
    public TextMeshProUGUI scoreText; // Text để hiện Best Score
    public TextMeshProUGUI latestScoreText; // Text để hiện Latest Score (vừa chơi xong)

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
        // Tải scene game
        ScoreManager.Instance.ResetCurrentScore();
        SceneManager.LoadScene(gameSceneName);
    }

    // Hàm này cho nút Continue
    public void ContinueGame()
    {
        // Tạm thời, nó cũng sẽ bắt đầu game mới
        // Sau này bạn sẽ thêm logic load game đã lưu ở đây
        Debug.Log("Chức năng Continue đang được phát triển!");
        SceneManager.LoadScene(gameSceneName);
    }

    // Hàm này cho nút Settings
    public void OpenSettings()
    {
        // Sau này bạn có thể tạo một panel cài đặt và bật nó ở đây
        Debug.Log("Mở màn hình cài đặt!");
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
}