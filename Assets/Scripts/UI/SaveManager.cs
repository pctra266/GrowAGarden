using System.Collections;
using System.Collections.Generic;
using System.IO; // Rất quan trọng: Thư viện để đọc và ghi file (File.WriteAllText, File.Exists...)
using UnityEngine;
using UnityEngine.Tilemaps; // Cần thiết vì chúng ta lưu/tải dữ liệu liên quan đến Tilemap

/// <summary>
/// Quản lý toàn bộ việc LƯU (Save) và TẢI (Load) dữ liệu game.
/// Đây là một "Singleton" (đối tượng duy nhất) để các script khác
/// có thể dễ dàng gọi Instance.SaveGame() hoặc Instance.LoadGame().
/// </summary>
public class SaveManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static SaveManager Instance;

    // Biến "gameData" này là "bình chứa" dữ liệu (ram)
    // Sẽ được điền vào khi Tải (Load) hoặc khi Lưu (Save)
    private GameData gameData;

    // Đường dẫn đầy đủ đến file save (ví dụ: C:/Users/TenBan/AppData/LocalLow/...)
    private string saveFilePath;

    [Header("!!! GÁN VÀO INSPECTOR")]
    [Tooltip("Gán ScriptableObject 'CowItem' vào đây")]
    public AnimalItem cowItemData; // Cần thiết để TÌM LẠI prefab Bò khi load
    [Tooltip("Gán ScriptableObject 'ChickenItem' vào đây")]
    public AnimalItem chickenItemData; // Cần thiết để TÌM LẠI prefab Gà khi load

    [Tooltip("GÁN PREFAB CỦA VẬT PHẨM RƠI RA (VÍ DỤ: Egg, Milk)")]
    public GameObject pickupItemPrefab; // <-- BIẾN MỚI RẤT QUAN TRỌNG
                                        // Dùng để TÁI TẠO lại vật phẩm rơi ra khi load game


    void Awake()
    {
        // --- Thiết lập Singleton Pattern ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ cho SaveManager tồn tại khi chuyển scene
        }
        else
        {
            Destroy(gameObject); // Nếu đã có 1 cái rồi, tự hủy bản sao này
            return;
        }
        // --- Kết thúc Singleton ---

        // Application.persistentDataPath là đường dẫn an toàn, chuẩn cho mọi nền tảng
        // (Windows, Mac, Android, iOS...) để lưu trữ dữ liệu người dùng.
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");

        // Tải dữ liệu từ file ngay khi game bắt đầu.
        // Dữ liệu này sẽ nằm "chờ" trong biến 'gameData'.
        LoadData();
    }

    // --- HÀM HỖ TRỢ ---

    /// <summary>
    /// Hàm "tra cứu" quan trọng.
    /// Tìm một ScriptableObject (Item) dựa vào TÊN (string) của nó.
    /// Cần thiết khi load game, vì file save chỉ lưu "Tên Item" chứ không lưu cả ScriptableObject.
    /// </summary>
    private Item FindItemByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        // Giả sử GameManager.instance.allItemsContainer chứa 1 danh sách
        // TẤT CẢ các item có thể có trong game (master list).
        foreach (ItemSlot slot in GameManager.instance.allItemsContainer.slots)
        {
            if (slot.item != null && slot.item.Name == name)
            {
                return slot.item; // Trả về ScriptableObject Item nếu tìm thấy
            }
        }
        Debug.LogWarning("SaveManager không tìm thấy item tên là: " + name);
        return null; // Không tìm thấy
    }

    /// <summary>
    /// Tương tự FindItemByName, nhưng dùng cho AnimalItem (Bò, Gà).
    /// Hàm này đang được "hard-code" (gán cứng) qua Inspector,
    /// nếu sau này có nhiều thú nuôi, bạn nên đổi sang một danh sách
    /// (List<AnimalItem>) để duyệt qua.
    /// </summary>
    private AnimalItem FindAnimalItemByName(string name)
    {
        if (cowItemData != null && name == cowItemData.Name) return cowItemData;
        if (chickenItemData != null && name == chickenItemData.Name) return chickenItemData;

        Debug.LogWarning("SaveManager không tìm thấy AnimalItem tên là: " + name);
        return null;
    }


    // --- HÀM LƯU GAME ---

    /// <summary>
    /// GOM TẤT CẢ dữ liệu từ các hệ thống (Manager) trong game
    /// và ĐƯA VÀO biến 'gameData'. Cuối cùng, ghi 'gameData' xuống file JSON.
    /// </summary>
    public void SaveGame()
    {
        // 1. DỮ LIỆU CƠ BẢN (Tiền, Ngày, Vị trí Người chơi)
        // (Lấy dữ liệu TỪ các script khác, gán VÀO 'gameData')
        gameData.money = MoneyController.money;
        gameData.day = DayController.dayCounter;
        if (GameManager.instance.player != null)
            gameData.playerPosition = GameManager.instance.player.transform.position;

        // 2. TÚI ĐỒ (INVENTORY)
        ItemContainer inventory = GameManager.instance.inventoryContainer;
        gameData.inventorySlots.Clear(); // Xóa sạch list cũ trước khi lưu
        foreach (ItemSlot slot in inventory.slots)
        {
            SerializableItemSlot savedSlot = new SerializableItemSlot();
            if (slot.item != null)
            {
                // Chỉ lưu TÊN và SỐ LƯỢNG
                savedSlot.itemName = slot.item.Name;
                savedSlot.count = slot.count;
            }
            // (Nếu slot.item == null, 'savedSlot' sẽ là 1 item rỗng (null, 0),
            // điều này là đúng để giữ đúng thứ tự các ô)
            gameData.inventorySlots.Add(savedSlot);
        }

        // 3. THỜI GIAN TRONG NGÀY
        // GHI CHÚ: FindObjectOfType<T>() có thể chậm nếu gọi liên tục.
        // Nhưng trong hàm SaveGame() (chỉ chạy 1 lần khi bấm save) thì hoàn toàn chấp nhận được.
        DayController dc = FindObjectOfType<DayController>();
        if (dc != null) gameData.timeElapsed = dc.timeElapsed;

        // 4. THỜI TIẾT / SỰ KIỆN
        gameData.runningEventName = ""; // Reset trước
        EventManager em = FindObjectOfType<EventManager>();
        if (em != null)
        {
            foreach (GameEvent e in em.gameEvents)
            {
                if (e.isRunning) // Nếu có 1 event đang chạy...
                {
                    gameData.runningEventName = e.eventName; // ...lưu tên nó lại
                    break; // và dừng tìm kiếm
                }
            }
        }

        // 5. THÚ NUÔI
        gameData.savedAnimals.Clear(); // Xóa list cũ
        AnimalController[] allAnimals = FindObjectsOfType<AnimalController>(); // Tìm TẤT CẢ thú nuôi trong Scene
        foreach (AnimalController animal in allAnimals)
        {
            SerializableAnimal sAnimal = new SerializableAnimal();
            sAnimal.position = animal.transform.position;
            sAnimal.productionTimer = animal.GetProductionTimer(); // Lấy thời gian sản xuất còn lại
            if (animal.GetAnimalData() != null)
                sAnimal.animalName = animal.GetAnimalData().Name; // Lưu tên (để biết là Bò hay Gà)

            gameData.savedAnimals.Add(sAnimal);
        }

        // 6. CÂY TRỒNG
        gameData.savedCrops.Clear();
        CropsManager cm = FindObjectOfType<CropsManager>();
        if (cm != null)
        {
            // Duyệt qua 'Dictionary' (hoặc List) các cây trồng mà CropsManager đang quản lý
            foreach (Crop crop in cm.crops.Values)
            {
                SerializableCrop sCrop = new SerializableCrop();
                sCrop.cropName = crop.name; // Tên (ID) của cây (ví dụ: "carrot_seed")
                sCrop.position = crop.position; // Vị trí ô (Grid)
                sCrop.currentStage = crop.currentStage; // Giai đoạn phát triển
                sCrop.timeRemaining = crop.timeRemaining; // Thời gian còn lại
                sCrop.timerIsRunning = crop.timerIsRunning; // Đã tưới nước hay chưa
                gameData.savedCrops.Add(sCrop);
            }
        }

        // 8. LƯU CÁC PREFAB ĐƯỢC SPAWN TRONG HARVESTPARENT (VD: Bụi cây dại, mỏ đá)
        gameData.savedHarvests.Clear();
        if (cm != null && cm.harvestParent != null) // cm đã được tìm ở (6)
        {
            // Duyệt qua tất cả các GameObject con của 'harvestParent'
            foreach (Transform child in cm.harvestParent)
            {
                GameObject go = child.gameObject;
                SerializableHarvest s = new SerializableHarvest();

                // Tên của prefab thường là "BushPrefab(Clone)". 
                // Chúng ta cần loại bỏ "(Clone)" để lấy tên gốc "BushPrefab".
                s.prefabName = go.name.Replace("(Clone)", "").Trim();
                s.position = go.transform.position;
                s.rotation = go.transform.rotation;
                gameData.savedHarvests.Add(s);
            }
        }


        // 7. --- MỚI: LƯU VẬT PHẨM RƠI RA (Pickups) ---
        gameData.savedPickups.Clear();
        PickUpItem[] allPickups = FindObjectsOfType<PickUpItem>(); // Tìm tất cả vật phẩm rơi ra
        foreach (PickUpItem pickup in allPickups)
        {
            // (Giả sử script PickUpItem của bạn có hàm GetItemData() trả về ScriptableObject Item)
            Item itemData = pickup.GetItemData();

            if (itemData != null)
            {
                SerializablePickup sPickup = new SerializablePickup();
                sPickup.itemName = itemData.Name; // Lưu tên (VD: "Egg", "Milk")
                sPickup.count = pickup.count; // Lưu số lượng
                sPickup.position = pickup.transform.position; // Lưu vị trí
                gameData.savedPickups.Add(sPickup);
            }
        }


        // 7.5: LƯU TRẠNG THÁI ĐẤT (ĐÃ CUỐC)
        gameData.plowedTiles.Clear();
        if (cm != null) // Tận dụng biến 'cm' đã tìm ở (6)
        {
            // Duyệt qua TẤT CẢ các ô trong phạm vi của tilemap
            // (Lưu ý: Cách này có thể chậm nếu bản đồ quá lớn)
            foreach (var pos in cm.groundTilemap.cellBounds.allPositionsWithin)
            {
                TileBase tile = cm.groundTilemap.GetTile(pos);

                // Nếu tile tại vị trí 'pos' là tile 'đất đã cuốc' (plowed)
                // VÀ ô đó không có cây trồng (được xử lý ở mục 6)
                // thì ta lưu vị trí 'pos' này lại.
                if (tile == cm.plowed)
                {
                    gameData.plowedTiles.Add(pos);
                }
            }
        }

        // 8. LƯU VÀO FILE
        // Chuyển toàn bộ đối tượng 'gameData' (và các list bên trong nó) thành 1 chuỗi JSON
        // 'true' ở cuối là để "pretty print" (format JSON cho đẹp, dễ đọc)
        string json = JsonUtility.ToJson(gameData, true);

        // Ghi chuỗi JSON đó vào file
        File.WriteAllText(saveFilePath, json);

        Debug.Log("ĐÃ LƯU GAME VÀO: " + saveFilePath);
    }

    // --- HÀM TẢI (LOAD) DỮ LIỆU ---

    /// <summary>
    /// Đọc dữ liệu từ file JSON và ĐƯA VÀO biến 'gameData'.
    /// Hàm này KHÔNG áp dụng dữ liệu vào game, mà chỉ "đọc file" thôi.
    /// (Hàm ApplyDataToGame() mới là hàm áp dụng)
    /// </summary>
    public void LoadData()
    {
        if (File.Exists(saveFilePath)) // Kiểm tra file save có tồn tại không
        {
            // Nếu có:
            string json = File.ReadAllText(saveFilePath); // Đọc toàn bộ file
            gameData = JsonUtility.FromJson<GameData>(json); // Chuyển JSON ngược lại thành object GameData
            Debug.Log("Đã tải game!");
        }
        else
        {
            // Nếu không có file save (lần đầu chơi):
            Debug.Log("Không tìm thấy file save, tạo game mới.");
            gameData = new GameData(); // Tạo 1 đối tượng GameData MỚI (với giá trị mặc định)
        }
    }

    // --- HÀM ÁP DỤNG DỮ LIỆU ĐÃ TẢI VÀO GAME ---

    /// <summary>
    /// Lấy dữ liệu TỪ 'gameData' và ÁP DỤNG (Apply) vào các
    /// hệ thống (Manager) trong Scene để TÁI TẠO lại trạng thái game.
    /// Hàm này thường được gọi SAU KHI Scene (màn chơi) đã được tải xong.
    /// </summary>
    public void ApplyDataToGame()
    {
        // Đề phòng trường hợp 'gameData' chưa được load (dù Awake() đã gọi LoadData())
        if (gameData == null) LoadData();

        // 1. TẢI DỮ LIỆU CƠ BẢN
        // (Lấy dữ liệu TỪ 'gameData', gán VÀO các script khác)
        MoneyController.money = gameData.money;
        DayController.dayCounter = gameData.day;
        if (GameManager.instance.player != null)
            GameManager.instance.player.transform.position = gameData.playerPosition;

        // 2. TẢI TÚI ĐỒ
        ItemContainer inventory = GameManager.instance.inventoryContainer;
        inventory.Clear(); // Xóa sạch túi đồ hiện tại

        // Đảm bảo số lượng ô (slot) trong inventory khớp với file save
        while (inventory.slots.Count < gameData.inventorySlots.Count)
            inventory.slots.Add(new ItemSlot());

        // Bắt đầu gán lại item
        for (int i = 0; i < gameData.inventorySlots.Count; i++)
        {
            SerializableItemSlot savedSlot = gameData.inventorySlots[i];
            if (savedSlot != null && !string.IsNullOrEmpty(savedSlot.itemName))
            {
                // Dùng hàm "tra cứu" để tìm ScriptableObject từ TÊN
                Item item = FindItemByName(savedSlot.itemName);
                if (item != null)
                {
                    // Gán lại item và số lượng vào ô
                    inventory.slots[i].Set(item, savedSlot.count);
                }
            }
            else
            {
                // Nếu file save nói ô này rỗng, ta dọn sạch nó
                inventory.slots[i].Clear();
            }
        }

        // 3. TẢI THỜI GIAN TRONG NGÀY
        DayController dc = FindObjectOfType<DayController>();
        if (dc != null) dc.timeElapsed = gameData.timeElapsed;

        // 4. TẢI THỜI TIẾT / SỰ KIỆN
        EventManager em = FindObjectOfType<EventManager>();
        if (em != null && !string.IsNullOrEmpty(gameData.runningEventName))
        {
            // Dừng event tự động (nếu có)
            if (em.autoTriggerCoroutine != null)
                em.StopCoroutine(em.autoTriggerCoroutine);

            // Kích hoạt lại event đã lưu
            em.TriggerEventByName(gameData.runningEventName);
        }

        // 5. TẢI THÚ NUÔI
        // BƯỚC A: Xóa TẤT CẢ thú nuôi cũ đang có trong Scene (nếu có)
        AnimalController[] oldAnimals = FindObjectsOfType<AnimalController>();
        foreach (var animal in oldAnimals) Destroy(animal.gameObject);

        // BƯỚC B: Tạo lại thú nuôi từ file save
        AnimalManager am = AnimalManager.Instance;
        if (am != null)
        {
            foreach (SerializableAnimal sAnimal in gameData.savedAnimals)
            {
                // Dùng hàm tra cứu để lấy AnimalItem (chứa prefab)
                AnimalItem itemData = FindAnimalItemByName(sAnimal.animalName);
                if (itemData != null)
                {
                    // Tạo (Instantiate) prefab thú nuôi
                    GameObject newAnimalObj = Instantiate(itemData.animalPrefab, sAnimal.position, Quaternion.identity);
                    AnimalController controller = newAnimalObj.GetComponent<AnimalController>();
                    if (controller != null)
                    {
                        // Thiết lập lại dữ liệu và bộ đếm thời gian
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
            cm.ClearAllCrops(); // (Giả sử hàm này cũng xóa (clear) các ô tile 'plowed' và 'watered')

            // BƯỚC B: Tải lại các ô đất 'Đã Cuốc' (Plowed)
            // (Phải chạy TRƯỚC khi tải cây)
            if (gameData.plowedTiles != null)
            {
                foreach (Vector3Int pos in gameData.plowedTiles)
                {
                    // Vẽ lại tile đất đã cuốc lên Tilemap
                    cm.groundTilemap.SetTile(pos, cm.plowed);
                }
            }

            // BƯỚC C: Tải lại cây trồng (sẽ đè lên đất đã cuốc)
            foreach (SerializableCrop sCrop in gameData.savedCrops)
            {
                // Gọi hàm của CropsManager để tái tạo cây trồng
                // (Hàm này sẽ tự xử lý việc đặt tile cây, tile nước...)
                cm.LoadCrop(sCrop);
            }
        }

        // --- 9. TẢI LẠI CÁC PREFAB HARVEST (Bụi cây, mỏ đá...) ---
        if (cm != null && cm.harvestParent != null) // cm đã được tìm ở (6)
        {
            // BƯỚC A: Xóa các harvest cũ
            List<Transform> oldHarvests = new List<Transform>();
            foreach (Transform child in cm.harvestParent)
                oldHarvests.Add(child);
            foreach (var t in oldHarvests)
                Destroy(t.gameObject); // Phá hủy

            // BƯỚC B: Tạo lại từ file save
            foreach (SerializableHarvest s in gameData.savedHarvests)
            {
                // Dùng 1 hàm tra cứu của CropsManager (dựa vào tên) để tìm lại prefab
                var info = cm.GetCropInfoByHarvestName(s.prefabName);
                if (info != null && info.harvestPrefab != null)
                {
                    // Tạo lại prefab đúng vị trí, xoay, và đặt làm con của 'harvestParent'
                    Instantiate(info.harvestPrefab, s.position, s.rotation, cm.harvestParent);
                }
                else
                {
                    Debug.LogWarning($"Không tìm thấy prefab harvest: {s.prefabName}");
                }
            }
        }

        // 7. --- MỚI: TẢI VẬT PHẨM RƠI RA (Pickups) ---
        // BƯỚC A: Xóa tất cả vật phẩm rơi ra cũ
        PickUpItem[] oldPickups = FindObjectsOfType<PickUpItem>();
        foreach (var pickup in oldPickups) Destroy(pickup.gameObject);

        // BƯỚC B: Kiểm tra xem đã gán prefab trên Inspector chưa
        if (pickupItemPrefab == null)
        {
            Debug.LogError("CHƯA GÁN 'pickupItemPrefab' TRÊN SAVEMANAGER!");
        }
        else
        {
            // BƯỚC C: Tạo lại vật phẩm rơi ra
            foreach (SerializablePickup sPickup in gameData.savedPickups)
            {
                // Tra cứu Item Data (ScriptableObject) từ tên
                Item itemData = FindItemByName(sPickup.itemName);
                if (itemData != null)
                {
                    // Tạo prefab vật phẩm tại vị trí đã lưu
                    GameObject obj = Instantiate(pickupItemPrefab, sPickup.position, Quaternion.identity);
                    PickUpItem pickupScript = obj.GetComponent<PickUpItem>();
                    if (pickupScript != null)
                    {
                        // Cấu hình cho vật phẩm này (nó là Trứng hay Sữa, số lượng bao nhiêu)
                        // (Giả sử bạn có hàm SetItem trong PickUpItem.cs)
                        pickupScript.SetItem(itemData, sPickup.count);
                    }
                }
            }
        }

        // --- CẬP NHẬT UI ---
        // Sau khi load xong, gọi 'Show()' (hoặc 'UpdateUI()')
        // để các panel UI (Túi đồ, Thanh công cụ) hiển thị đúng
        Debug.Log("ĐÃ ÁP DỤNG DỮ LIỆU VÀO GAME.");

        InventoryPanel invPanel = FindObjectOfType<InventoryPanel>();
        ItemToolbarPanel toolbarPanel = FindObjectOfType<ItemToolbarPanel>();
        if (invPanel != null) invPanel.Show(); // Cập nhật UI túi đồ
        if (toolbarPanel != null) toolbarPanel.Show(); // Cập nhật UI thanh công cụ
    }

    /// <summary>
    /// Kiểm tra xem đã có file save hay chưa.
    /// Dùng cho Main Menu để quyết định hiển thị nút "Continue" hay không.
    /// </summary>
    public bool HasSaveData()
    {
        return File.Exists(saveFilePath);
    }


    // --- HÀM QUẢN LÝ VẬT THỂ BỊ PHÁ HỦY (Destroyed Objects) ---

    /// <summary>
    /// Ghi lại ID của một vật thể đã bị phá hủy vĩnh viễn (ví dụ: đá, cây lớn).
    /// </summary>
    public void RecordDestroyedObject(string id)
    {
        if (gameData.destroyedObjectIDs == null)
        {
            gameData.destroyedObjectIDs = new List<string>();
        }

        // Chỉ thêm nếu ID này chưa có trong danh sách
        if (!gameData.destroyedObjectIDs.Contains(id))
        {
            gameData.destroyedObjectIDs.Add(id);
            Debug.Log("Đã ghi nhận phá hủy: " + id);
        }
    }

    /// <summary>
    /// Kiểm tra xem một vật thể (dựa vào ID) đã bị phá hủy trước đó chưa.
    /// Các vật thể (đá, cây) sẽ gọi hàm này trong hàm Start() của chúng
    /// để quyết định xem chúng có nên tự hủy (Destroy(gameObject)) hay không.
    /// </summary>
    public bool IsObjectDestroyed(string id)
    {
        if (gameData.destroyedObjectIDs == null)
        {
            gameData.destroyedObjectIDs = new List<string>();
            return false; // List rỗng, chắc chắn chưa bị phá
        }

        return gameData.destroyedObjectIDs.Contains(id);
    }

    /// <summary>
    /// Xóa sạch danh sách vật thể bị phá hủy.
    /// Được gọi khi người chơi chọn "New Game" (Chơi mới).
    /// </summary>
    public void ResetDestroyedObjectsList()
    {
        if (gameData.destroyedObjectIDs == null)
        {
            gameData.destroyedObjectIDs = new List<string>();
        }
        else
        {
            gameData.destroyedObjectIDs.Clear();
        }
        Debug.Log("Đã reset danh sách vật thể bị phá hủy (New Game).");
    }
}