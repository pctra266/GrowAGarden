using UnityEngine;
[CreateAssetMenu(fileName = "New Animal Item", menuName = "Data/Animal Item", order = 1)]
public class AnimalItem : Item
{

    public GameObject animalPrefab;
    public float movementSpeed = 1.0f;

    [Header("Production")]
    public Item productItem;
    public float productionTimeInSeconds = 120f;
    public int purchaseCost = 250;
}
