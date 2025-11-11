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
    public float dayDuration = 120f; // Duration of a full day-night cycle in seconds
    [SerializeField]
    private TextMeshProUGUI dayCounterText;
    [SerializeField]
    private TextMeshProUGUI timeOfDayText;
    // NOTE: Do NOT assign UI objects from other scenes to this script. Instead we set a flag
    // to tell the Main Menu to open the Best Score panel after loading.
    public int targetDayToShowBest = 30;
    private bool hasShownBestScore = false;
    // Delay in seconds to show the end-of-run message before returning to menu
    public float returnToMenuDelay = 1f;
    // Scene name to load when returning to main menu
    public string mainMenuSceneName = "MainMenu";
    private float timeElapsed;
    private float distanceBetweenSunAndMoon = 46f;
    private int dayCounter = 0;
    private List<GameObject> lights;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeElapsed = dayDuration / 4;
        lights = new List<GameObject> { sun, moon };
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        float xPosition = lights[0].transform.position.x - (2*distanceBetweenSunAndMoon / dayDuration) * Time.deltaTime;
        lights[0].transform.position = new Vector3(xPosition, lights[0].transform.position.y, lights[0].transform.position.z);
        xPosition = lights[1].transform.position.x - (2*distanceBetweenSunAndMoon / dayDuration) * Time.deltaTime;
        lights[1].transform.position = new Vector3(xPosition, lights[1].transform.position.y, lights[1].transform.position.z);
        if (lights[0].transform.position.x <= threshold.transform.position.x)
        {
            lights[1].transform.position = new Vector3(threshold.transform.position.x + distanceBetweenSunAndMoon, lights[1].transform.position.y, lights[1].transform.position.z);
            lights.Insert(0, lights[1]);
            lights.RemoveAt(2);
        }
        if (timeOfDayText != null)
        {
            timeOfDayText.text = Mathf.FloorToInt(((timeElapsed / dayDuration) * 24)).ToString() + ":00";
        }
        
        if (timeElapsed >= dayDuration)
        {
            timeElapsed = 0f; // Reset the cycle
            dayCounter++;
            if (dayCounterText != null)
            {
                dayCounterText.text = "Day: " + dayCounter.ToString();
            }
            // Check target day and show Best Score UI once
            if (!hasShownBestScore && dayCounter >= targetDayToShowBest)
            {
                hasShownBestScore = true;

                // Use ScoreManager's currentScore (score is accumulated at sales only).
                int current = 0;
                if (ScoreManager.Instance != null)
                {
                    current = ScoreManager.Instance.currentScore;
                    ScoreManager.Instance.TrySetNewBest(current);
                }
                else
                {
                    // Fallback: read previously stored LatestScore (updated by MoneyController fallback)
                    current = PlayerPrefs.GetInt("LatestScore", 0);
                    int bestSoFar = PlayerPrefs.GetInt("BestScore", 0);
                    if (current > bestSoFar)
                    {
                        PlayerPrefs.SetInt("BestScore", current);
                        PlayerPrefs.Save();
                    }
                }

                // Set a flag so MainMenu will open the Best Score panel when it loads
                PlayerPrefs.SetInt("ShowBestOnMenu", 1);
                PlayerPrefs.SetInt("LatestScore", current);
                PlayerPrefs.Save();

                // Start coroutine to return to menu after delay (use realtime so it's independent of timeScale)
                StartCoroutine(EndRunAndReturn(returnToMenuDelay));
            }
        }
        
    }

    private System.Collections.IEnumerator EndRunAndReturn(float delay)
    {
        // Optionally ensure the game is paused visually
        Time.timeScale = 0f;

        float elapsed = 0f;
        while (elapsed < delay)
        {
            // use unscaled delta so UI timers still progress if timeScale == 0
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Restore timeScale and load main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
