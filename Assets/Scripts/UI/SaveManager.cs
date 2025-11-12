using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    private GameData gameData;
    private string saveFilePath;

    [Header("!!! GÁN VÀO INSPECTOR")]
    [Tooltip("Gán ScriptableObject 'CowItem' vào đây")]
    public AnimalItem cowItemData;
    [Tooltip("Gán ScriptableObject 'ChickenItem' vào đây")]
    public AnimalItem chickenItemData;

    [Tooltip("GÁN PREFAB CỦA VẬT PHẨM RƠI RA (VÍ DỤ: Egg, Milk)")]
    public GameObject pickupItemPrefab; // <-- BIẾN MỚI RẤT QUAN TRỌNG


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        LoadData();
    }

    // --- HÀM HỖ TRỢ ---

    private Item FindItemByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        foreach (ItemSlot slot in GameManager.instance.allItemsContainer.slots)
        {
            if (slot.item != null && slot.item.Name == name)
            {
                return slot.item;
            }
        }
        Debug.LogWarning("SaveManager không tìm thấy item tên là: " + name);
        return null;
    }

    private AnimalItem FindAnimalItemByName(string name)
    {
        if (cowItemData != null && name == cowItemData.Name) return cowItemData;
        if (chickenItemData != null && name == chickenItemData.Name) return chickenItemData;
        Debug.LogWarning("SaveManager không tìm thấy AnimalItem tên là: " + name);
        return null;
    }


    // --- HÀM LƯU GAME ---
    public void SaveGame()
    {
        // 1. DỮ LIỆU CƠ BẢN
        gameData.money = MoneyController.money;
        gameData.day = DayController.dayCounter;
        if (GameManager.instance.player != null)
            gameData.playerPosition = GameManager.instance.player.transform.position;

        // 2. TÚI ĐỒ
        ItemContainer inventory = GameManager.instance.inventoryContainer;
        gameData.inventorySlots.Clear();
        foreach (ItemSlot slot in inventory.slots)
        {
            SerializableItemSlot savedSlot = new SerializableItemSlot();
            if (slot.item != null)
            {
                savedSlot.itemName = slot.item.Name;
                savedSlot.count = slot.count;
            }
            gameData.inventorySlots.Add(savedSlot);
        }

        // 3. THỜI GIAN TRONG NGÀY
        DayController dc = FindObjectOfType<DayController>();
        if (dc != null) gameData.timeElapsed = dc.timeElapsed;

        // 4. THỜI TIẾT
        gameData.runningEventName = "";
        EventManager em = FindObjectOfType<EventManager>();
        if (em != null)
        {
            foreach (GameEvent e in em.gameEvents)
            {
                if (e.isRunning)
                {
                    gameData.runningEventName = e.eventName;
                    break;
                }
            }
        }

        // 5. THÚ NUÔI
        gameData.savedAnimals.Clear();
        AnimalController[] allAnimals = FindObjectsOfType<AnimalController>();
        foreach (AnimalController animal in allAnimals)
        {
            SerializableAnimal sAnimal = new SerializableAnimal();
            sAnimal.position = animal.transform.position;
            sAnimal.productionTimer = animal.GetProductionTimer();
            if (animal.GetAnimalData() != null)
                sAnimal.animalName = animal.GetAnimalData().Name;
            gameData.savedAnimals.Add(sAnimal);
        }

        // 6. CÂY TRỒNG
        gameData.savedCrops.Clear();
        CropsManager cm = FindObjectOfType<CropsManager>();
        if (cm != null)
        {
            foreach (Crop crop in cm.crops.Values)
            {
                SerializableCrop sCrop = new SerializableCrop();
                sCrop.cropName = crop.name;
                sCrop.position = crop.position;
                sCrop.currentStage = crop.currentStage;
                sCrop.timeRemaining = crop.timeRemaining;
                sCrop.timerIsRunning = crop.timerIsRunning;
                gameData.savedCrops.Add(sCrop);
            }
        }
        // --- 8. LƯU CÁC PREFAB ĐƯỢC SPAWN TRONG HARVESTPARENT ---
        gameData.savedHarvests.Clear();

        if (cm != null && cm.harvestParent != null)
        {
            foreach (Transform child in cm.harvestParent)
            {
                GameObject go = child.gameObject;
                SerializableHarvest s = new SerializableHarvest();
                s.prefabName = go.name.Replace("(Clone)", "").Trim(); // lưu tên gốc
                s.position = go.transform.position;
                s.rotation = go.transform.rotation;
                gameData.savedHarvests.Add(s);
            }
        }


        // 7. --- MỚI: LƯU VẬT PHẨM RƠI RA ---
        gameData.savedPickups.Clear();
        PickUpItem[] allPickups = FindObjectsOfType<PickUpItem>();
        foreach (PickUpItem pickup in allPickups)
        {
            // Chúng ta cần lấy Item Data từ script PickUpItem
            // (Giả sử bạn có một hàm GetItem() hoặc biến public 'item')
            // DỰA VÀO CODE 'PickUpItem.cs' CỦA BẠN: BẠN CẦN SỬA NÓ MỘT CHÚT

            // HÃY VÀO SCRIPT `PickUpItem.cs` VÀ THÊM DÒNG NÀY:
            // public Item GetItemData() { return item; }

            Item itemData = pickup.GetItemData(); // <-- Gọi hàm bạn vừa thêm

            if (itemData != null)
            {
                SerializablePickup sPickup = new SerializablePickup();
                sPickup.itemName = itemData.Name;
                sPickup.count = pickup.count;
                sPickup.position = pickup.transform.position;
                gameData.savedPickups.Add(sPickup);
            }
        }


        // --- 7.5: LƯU TRẠNG THÁI ĐẤT (ĐÃ CUỐC) ---
        gameData.plowedTiles.Clear();
        if (cm != null) // cm đã được tìm thấy ở Phần 6
        {
            // Duyệt qua tất cả các ô trong giới hạn của tilemap
            foreach (var pos in cm.groundTilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = cm.groundTilemap.GetTile(pos);

                // Nếu là ô đã cuốc (nhưng chưa có cây) thì lưu lại
                // (Chúng ta chỉ cần lưu 'plowed', vì cây trồng sẽ tự xử lý 'watered')
                if (tile == cm.plowed)
                {
                    gameData.plowedTiles.Add(pos);
                }
            }
        }

        // 8. LƯU VÀO FILE
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("ĐÃ LƯU GAME VÀO: " + saveFilePath);
    }

    // --- HÀM TẢI (LOAD) DỮ LIỆU ---
    public void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Đã tải game!");
        }
        else
        {
            Debug.Log("Không tìm thấy file save, tạo game mới.");
            gameData = new GameData();
        }
    }

    // --- HÀM ÁP DỤNG DỮ LIỆU ĐÃ TẢI VÀO GAME ---
    public void ApplyDataToGame()
    {
        if (gameData == null) LoadData();

        // 1. TẢI DỮ LIỆU CƠ BẢN
        MoneyController.money = gameData.money;
        DayController.dayCounter = gameData.day;
        if (GameManager.instance.player != null)
            GameManager.instance.player.transform.position = gameData.playerPosition;

        // 2. TẢI TÚI ĐỒ
        ItemContainer inventory = GameManager.instance.inventoryContainer;
        inventory.Clear();
        while (inventory.slots.Count < gameData.inventorySlots.Count)
            inventory.slots.Add(new ItemSlot());

        for (int i = 0; i < gameData.inventorySlots.Count; i++)
        {
            SerializableItemSlot savedSlot = gameData.inventorySlots[i];
            if (savedSlot != null && !string.IsNullOrEmpty(savedSlot.itemName))
            {
                Item item = FindItemByName(savedSlot.itemName);
                if (item != null)
                    inventory.slots[i].Set(item, savedSlot.count);
            }
            else
            {
                inventory.slots[i].Clear();
            }
        }

        // 3. TẢI THỜI GIAN TRONG NGÀY
        DayController dc = FindObjectOfType<DayController>();
        if (dc != null) dc.timeElapsed = gameData.timeElapsed;

        // 4. TẢI THỜI TIẾT
        EventManager em = FindObjectOfType<EventManager>();
        if (em != null && !string.IsNullOrEmpty(gameData.runningEventName))
        {
            if (em.autoTriggerCoroutine != null)
                em.StopCoroutine(em.autoTriggerCoroutine);

            em.TriggerEventByName(gameData.runningEventName);
        }

        // 5. TẢI THÚ NUÔI
        AnimalController[] oldAnimals = FindObjectsOfType<AnimalController>();
        foreach (var animal in oldAnimals) Destroy(animal.gameObject);

        AnimalManager am = AnimalManager.Instance;
        if (am != null)
        {
            foreach (SerializableAnimal sAnimal in gameData.savedAnimals)
            {
                AnimalItem itemData = FindAnimalItemByName(sAnimal.animalName);
                if (itemData != null)
                {
                    GameObject newAnimalObj = Instantiate(itemData.animalPrefab, sAnimal.position, Quaternion.identity);
                    AnimalController controller = newAnimalObj.GetComponent<AnimalController>();
                    if (controller != null)
                    {
                        controller.Initialize(itemData);
                        controller.SetProductionTimer(sAnimal.productionTimer);
                    }
                }
            }
        }

        // --- 6. TẢI CÂY TRỒNG & ĐẤT ---
        CropsManager cm = FindObjectOfType<CropsManager>();
        if (cm != null)
        {
            // BƯỚC A: Xóa sạch mọi thứ cũ (cả cây và đất)
            cm.ClearAllCrops();

            // BƯỚC B: Tải lại các ô đất 'Đã Cuốc' (Plowed)
            // (Phải chạy TRƯỚC khi tải cây)
            if (gameData.plowedTiles != null)
            {
                foreach (Vector3Int pos in gameData.plowedTiles)
                {
                    // Đặt lại tile đất đã cuốc
                    cm.groundTilemap.SetTile(pos, cm.plowed);
                }
            }

            // BƯỚC C: Tải lại cây trồng (sẽ đè lên đất đã cuốc)
            foreach (SerializableCrop sCrop in gameData.savedCrops)
            {
                cm.LoadCrop(sCrop);
            }
        }
        // --- 9. TẢI LẠI CÁC PREFAB HARVEST ---
        if (cm != null && cm.harvestParent != null)
        {
            // Xóa các harvest cũ
            List<Transform> oldHarvests = new List<Transform>();
            foreach (Transform child in cm.harvestParent)
                oldHarvests.Add(child);
            foreach (var t in oldHarvests)
                Destroy(t.gameObject);

            // Tạo lại
            foreach (SerializableHarvest s in gameData.savedHarvests)
            {
                // tìm prefab tương ứng trong cropList của CropsManager
                var info = cm.GetCropInfoByHarvestName(s.prefabName);
                if (info != null && info.harvestPrefab != null)
                {
                    Instantiate(info.harvestPrefab, s.position, s.rotation, cm.harvestParent);
                }
                else
                {
                    Debug.LogWarning($"Không tìm thấy prefab harvest: {s.prefabName}");
                }
            }
        }





        // 7. --- MỚI: TẢI VẬT PHẨM RƠI RA ---
        PickUpItem[] oldPickups = FindObjectsOfType<PickUpItem>();
        foreach (var pickup in oldPickups) Destroy(pickup.gameObject);

        if (pickupItemPrefab == null)
        {
            Debug.LogError("CHƯA GÁN 'pickupItemPrefab' TRÊN SAVEMANAGER!");
        }
        else
        {
            foreach (SerializablePickup sPickup in gameData.savedPickups)
            {
                Item itemData = FindItemByName(sPickup.itemName);
                if (itemData != null)
                {
                    GameObject obj = Instantiate(pickupItemPrefab, sPickup.position, Quaternion.identity);
                    PickUpItem pickupScript = obj.GetComponent<PickUpItem>();
                    if (pickupScript != null)
                    {
                        pickupScript.SetItem(itemData, sPickup.count);
                    }
                }
            }
        }


        Debug.Log("ĐÃ ÁP DỤNG DỮ LIỆU VÀO GAME.");

        InventoryPanel invPanel = FindObjectOfType<InventoryPanel>();
        ItemToolbarPanel toolbarPanel = FindObjectOfType<ItemToolbarPanel>();
        if (invPanel != null) invPanel.Show();
        if (toolbarPanel != null) toolbarPanel.Show();
    }

    public bool HasSaveData()
    {
        return File.Exists(saveFilePath);
    }


    // --- HÀM MỚI ĐỂ GHI LẠI VẬT BỊ PHÁ HỦY ---
    public void RecordDestroyedObject(string id)
    {
        if (gameData.destroyedObjectIDs == null)
        {
            gameData.destroyedObjectIDs = new List<string>();
        }

        if (!gameData.destroyedObjectIDs.Contains(id))
        {
            gameData.destroyedObjectIDs.Add(id);
            Debug.Log("Đã ghi nhận phá hủy: " + id);
        }
    }

    // --- HÀM MỚI ĐỂ KIỂM TRA VẬT BỊ PHÁ HỦY ---
    public bool IsObjectDestroyed(string id)
    {
        if (gameData.destroyedObjectIDs == null)
        {
            gameData.destroyedObjectIDs = new List<string>();
            return false;
        }

        return gameData.destroyedObjectIDs.Contains(id);
    }
}
