using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance { get; private set; }

    [Header("Configuration")]
    public Transform spawnPoint;
    public int maxAnimals = 10;

    public GameObject productPickupPrefab;

    private List<GameObject> activeAnimals = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public bool CanPurchaseAnimal()
    {
        return activeAnimals.Count < maxAnimals;
    }

    public void PurchaseAnimal(AnimalItem animalItem)
    {
        if (!CanPurchaseAnimal())
        {
            Debug.Log("Nông trại đã đầy!");
            return;
        }

        if (animalItem.animalPrefab == null)
        {
            Debug.LogError("Animal Prefab chưa được gán trong " + animalItem.Name);
            return;
        }

        GameObject newAnimalObj = Instantiate(animalItem.animalPrefab, spawnPoint.position, Quaternion.identity);

        AnimalController controller = newAnimalObj.GetComponent<AnimalController>();
        if (controller == null)
        {
            controller = newAnimalObj.AddComponent<AnimalController>();
        }

        controller.Initialize(animalItem);

        activeAnimals.Add(newAnimalObj);
    }
}