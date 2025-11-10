using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Rất quan trọng để chuyển scene

public class PauseManager : MonoBehaviour
{
    // Biến này để kiểm tra xem game có đang tạm dừng hay không
    public static bool isGamePaused = false;

    // Kéo Panel PauseMenu vào đây trong Inspector
    public GameObject pauseMenuUI;

    // Gán tên Scene Main Menu của bạn
    public string mainMenuSceneName = "MainMenu";

    // Update được gọi mỗi khung hình
    void Update()
    {
        // Nếu người chơi nhấn phím Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                // Nếu đang pause thì resume
                Resume();
            }
            else
            {
                // Nếu không pause thì pause
                Pause();
            }
        }
    }

    // Hàm để tiếp tục game
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Ẩn menu đi
        Time.timeScale = 1f; // Cho thời gian chạy lại bình thường
        isGamePaused = false;
    }

    // Hàm để tạm dừng game
    void Pause()
    {
        pauseMenuUI.SetActive(true); // Hiện menu lên
        Time.timeScale = 0f; // ĐÓNG BĂNG thời gian của game
        isGamePaused = true;
    }

    // Hàm để quay về Main Menu
    public void LoadMenu()
    {
        // Rất quan trọng: Phải cho thời gian chạy lại trước khi chuyển scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Hàm để thoát game
    public void QuitGame()
    {
        Debug.Log("Thoát Game!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}