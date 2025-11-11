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
    public void LoadMenu()
    {
        // --- THÊM LOGIC LƯU ĐIỂM ---
        SaveBestScore();
        // --- KẾT THÚC LOGIC LƯU ĐIỂM ---

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        // --- THÊM LOGIC LƯU ĐIỂM ---
        SaveBestScore();
        // --- KẾT THÚC LOGIC LƯU ĐIỂM ---

        Debug.Log("Thoát Game!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }



    void SaveBestScore()
    {
        // Lấy số vàng hiện tại trực tiếp từ script MoneyController
        // Vì 'money' là static, ta gọi thẳng: MoneyController.money
        long currentGold = MoneyController.money;

        // Lấy điểm cao nhất đã lưu (dưới dạng 'int')
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        // PlayerPrefs không lưu được 'long', nên ta cần chuyển về 'int'.
        // Dùng Mathf.Clamp để đảm bảo an toàn nếu số vàng quá lớn.
        int currentGoldAsInt = Mathf.Clamp((int)currentGold, int.MinValue, int.MaxValue);

        // So sánh điểm
        if (currentGoldAsInt > bestScore)
        {
            // Nếu điểm (vàng) mới cao hơn, lưu lại
            PlayerPrefs.SetInt("BestScore", currentGoldAsInt);
            PlayerPrefs.Save(); // Lưu thay đổi
            Debug.Log("Đã lưu Best Score mới: " + currentGoldAsInt);
        }
    }


    // Hàm này sẽ được gọi bởi nút "Save and Quit"
    public void SaveAndQuitToMenu()
    {
        // 1. Gọi SaveManager để lưu game
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }

        // 2. Quay về menu (giống hàm LoadMenu cũ)
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}