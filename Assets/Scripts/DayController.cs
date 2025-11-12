using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DayController : MonoBehaviour
{
    public GameObject sun;
    public GameObject moon;
    public GameObject threshold;
    public float dayDuration = 120f;
    [SerializeField]
    private TextMeshProUGUI dayCounterText;
    [SerializeField]
    private TextMeshProUGUI timeOfDayText;
    public int targetDayToShowBest = 30;
    private bool hasShownBestScore = false;
    public float returnToMenuDelay = 1f;
    public string mainMenuSceneName = "MainMenu";

    public float timeElapsed;
    private float distanceBetweenSunAndMoon = 46f;
    public static int dayCounter = 0;
    private List<GameObject> lights;

    // --- BIẾN MỚI ĐỂ LƯU VỊ TRÍ GỐC ---
    private Vector3 sunStartPos;
    private Vector3 moonStartPos;
    private float cycleSpeed;

    void Awake()
    {
        // --- LƯU LẠI VỊ TRÍ GỐC (TRONG EDITOR) CỦA BẠN ---
        // Phải chạy trước Start()
        sunStartPos = sun.transform.position;
        moonStartPos = moon.transform.position;
        lights = new List<GameObject>();
        cycleSpeed = (2 * distanceBetweenSunAndMoon / dayDuration);
    }

    void Start()
    {
        // Logic này sẽ quyết định tải game hay chơi mới
        if (MainMenuManager.IsLoadingGame == true)
        {
            // --- TẢI GAME ---
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.ApplyDataToGame();
            }
        }
        else
        {
            // --- GAME MỚI ---
            MoneyController.money = 300;
            DayController.dayCounter = 0;
            timeElapsed = dayDuration / 4; // Đặt về 6:00

            if (GameManager.instance != null && GameManager.instance.inventoryContainer != null)
                GameManager.instance.inventoryContainer.Clear();

            CropsManager cm = FindObjectOfType<CropsManager>();
            if (cm != null) cm.ClearAllCrops();

            AnimalController[] oldAnimals = FindObjectsOfType<AnimalController>();
            foreach (var animal in oldAnimals) Destroy(animal.gameObject);
        }

        // --- HÀM MỚI QUAN TRỌNG ---
        // Dựa trên timeElapsed (mới hoặc đã tải), đặt lại vị trí
        SetSunMoonState(timeElapsed);

        // Cập nhật UI ngay lập tức
        if (dayCounterText != null)
        {
            dayCounterText.text = "Day: " + dayCounter.ToString();
        }
        if (timeOfDayText != null)
        {
            timeOfDayText.text = Mathf.FloorToInt(((timeElapsed / dayDuration) * 24)).ToString() + ":00";
        }
    }

    // --- HÀM MỚI ĐỂ CÀI ĐẶT CẢNH VẬT ---
    void SetSunMoonState(float currentTime)
    {
        lights.Clear();

        float quarterDay = dayDuration / 4; // 6:00
        float threeQuarterDay = dayDuration * 3 / 4; // 18:00

        // KIỂM TRA XEM LÀ BAN NGÀY HAY BAN ĐÊM
        // Giả sử: Ngày = 6:00 (quarter) đến 18:00 (threeQuarter)
        if (currentTime >= quarterDay && currentTime < threeQuarterDay)
        {
            // --- ĐANG LÀ BAN NGÀY ---
            lights.Add(sun);  // Sun chạy (index 0)
            lights.Add(moon); // Moon chờ (index 1)

            float timeIntoDay = currentTime - quarterDay; // Thời gian trôi qua từ 6:00

            // Đặt Sun vào đúng vị trí
            sun.transform.position = new Vector3(sunStartPos.x - (cycleSpeed * timeIntoDay), sunStartPos.y, sunStartPos.z);
            // Đặt Moon vào đúng vị trí
            moon.transform.position = new Vector3(moonStartPos.x - (cycleSpeed * timeIntoDay), moonStartPos.y, moonStartPos.z);
        }
        else
        {
            // --- ĐANG LÀ BAN ĐÊM ---
            lights.Add(moon); // Moon chạy (index 0)
            lights.Add(sun);  // Sun chờ (index 1)

            float timeIntoNight;
            if (currentTime >= threeQuarterDay) // Từ 18:00 đến 00:00
            {
                timeIntoNight = currentTime - threeQuarterDay;
            }
            else // Từ 00:00 đến 6:00
            {
                timeIntoNight = currentTime + quarterDay;
            }

            // Đặt Moon vào đúng vị trí (nó bắt đầu từ sunStartPos)
            moon.transform.position = new Vector3(sunStartPos.x - (cycleSpeed * timeIntoNight), sunStartPos.y, sunStartPos.z);
            // Đặt Sun vào đúng vị trí (nó bắt đầu từ moonStartPos)
            sun.transform.position = new Vector3(moonStartPos.x - (cycleSpeed * timeIntoNight), moonStartPos.y, moonStartPos.z);
        }
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        // Di chuyển
        float xPosition = lights[0].transform.position.x - cycleSpeed * Time.deltaTime;
        lights[0].transform.position = new Vector3(xPosition, lights[0].transform.position.y, lights[0].transform.position.z);
        xPosition = lights[1].transform.position.x - cycleSpeed * Time.deltaTime;
        lights[1].transform.position = new Vector3(xPosition, lights[1].transform.position.y, lights[1].transform.position.z);

        // Hoán đổi
        if (lights[0].transform.position.x <= threshold.transform.position.x)
        {
            lights[1].transform.position = new Vector3(threshold.transform.position.x + distanceBetweenSunAndMoon, lights[1].transform.position.y, lights[1].transform.position.z);
            lights.Insert(0, lights[1]);
            lights.RemoveAt(2);
        }

        // Cập nhật UI
        if (timeOfDayText != null)
        {
            timeOfDayText.text = Mathf.FloorToInt(((timeElapsed / dayDuration) * 24)).ToString() + ":00";
        }

        // Chuyển ngày
        if (timeElapsed >= dayDuration)
        {
            timeElapsed = 0f; // Reset the cycle
            dayCounter++;
            if (dayCounterText != null)
            {
                dayCounterText.text = "Day: " + dayCounter.ToString();
            }

            if (!hasShownBestScore && dayCounter >= targetDayToShowBest)
            {
                hasShownBestScore = true;
                PlayerPrefs.SetInt("ShowBestOnMenu", 1);
                StartCoroutine(EndRunAndReturn(returnToMenuDelay));
            }
        }
    }

    private System.Collections.IEnumerator EndRunAndReturn(float delay)
    {
        Time.timeScale = 0f;
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}