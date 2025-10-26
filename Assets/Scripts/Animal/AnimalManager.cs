using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    // Singleton Pattern để cửa hàng có thể dễ dàng gọi đến
    public static AnimalManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // Hàm duy nhất mà cửa hàng cần biết
    // Nó nhận vào DỮ LIỆU của con vật và vị trí để tạo ra nó
    public void PurchaseAnimal(AnimalItem animalItem, Vector3 spawnPosition)
    {
        if (animalItem.animalPrefab == null)
        {
            Debug.LogError("Animal Prefab chưa được gán trong " + animalItem.Name);
            return;
        }

        // Tạo một bản sao của prefab tương ứng (CowPrefab hoặc ChickenPrefab) tại vị trí được chỉ định
        GameObject newAnimalObj = Instantiate(animalItem.animalPrefab, spawnPosition, Quaternion.identity);

        // Lấy "bộ não" (AnimalController) từ con vật vừa tạo
        AnimalController controller = newAnimalObj.GetComponent<AnimalController>();

        if (controller != null)
        {
            // "Nạp" dữ liệu vào bộ não.
            controller.Initialize(animalItem);
        }
    }
}