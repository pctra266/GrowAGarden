using UnityEngine;
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
        }
        
    }
}
