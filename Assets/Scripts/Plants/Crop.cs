using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Globalization;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Data/Crop")]

public class Crop : ScriptableObject
{
    public TileBase state0, state1, state2, state3, state4, state5;
    public TileBase state;
    public Vector3Int position;
    public float timeRemaining;
    public bool timerIsRunning;
    public int currentStage = 0;

    public void NextState()
    {
        currentStage++;
        switch (currentStage)
        {
            case 1: state = state1; break;
            case 2: state = state2; break;
            case 3: state = state3; break;
            case 4: state = state4; break;
            case 5: state = state5; break;
        }
    }
}