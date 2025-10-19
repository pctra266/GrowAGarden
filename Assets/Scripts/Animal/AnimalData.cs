using UnityEngine;

public class AnimalData : ScriptableObject
{
    public string animalName;
    public float moveSpeed = 1f;
    public float wanderRadius = 3f;

    public float minIdleTime = 2f;
    public float maxIdleTime = 5f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 6f;

    public float productionTime = 20f;
    public GameObject productPrefab;
    public RuntimeAnimatorController animatorController;
}
