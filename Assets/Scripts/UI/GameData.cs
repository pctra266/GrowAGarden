using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// --- GHI CHÚ QUAN TRỌNG ---
// [System.Serializable]
// Đây là một "Attribute" (thuộc tính) cực kỳ quan trọng.
// Nó báo cho Unity (và các trình tuần tự hóa khác) rằng "Hãy cho phép các đối tượng
// thuộc class này được chuyển đổi thành các định dạng như JSON hoặc file nhị phân".
// Nếu không có nó, các class này sẽ không thể được lưu lại.

/// <summary>
/// Class phụ để lưu trữ dữ liệu của một ô trong túi đồ (Inventory).
/// Lý do dùng class này: Không thể lưu trực tiếp ScriptableObject (ItemData) vào JSON.
/// Thay vào đó, chúng ta lưu tên (string) của item và dùng nó để tìm lại 
/// ScriptableObject tương ứng khi load game.
/// </summary>
[System.Serializable]
public class SerializableItemSlot
{
    // Tên của ScriptableObject (ItemData) để dùng làm khóa tra cứu khi load game.
    // Ví dụ: "Hạt Cà Rốt", "Cuốc Sắt"
    public string itemName;
    // Số lượng item trong ô này.
    public int count;
}

/// <summary>
/// Class phụ để lưu trạng thái của một con thú nuôi (Animal) trong trang trại.
/// </summary>
[System.Serializable]
public class SerializableAnimal
{
    // Tên của Prefab hoặc ScriptableObject đại diện cho con vật.
    // Dùng để biết cần spawn lại con "Bò" hay "Gà" khi load game.
    public string animalName;

    // Vị trí (x, y, z) cuối cùng của con vật trước khi save.
    public Vector3 position;

    // Bộ đếm thời gian sản xuất (ví dụ: còn bao lâu nữa thì cho sữa/trứng).
    // Khi load game, bộ đếm này sẽ được khôi phục và tiếp tục chạy.
    public float productionTimer;
}

/// <summary>
/// Class phụ để lưu trạng thái của một cây trồng (Crop) trên các ô đất.
/// </summary>
[System.Serializable]
public class SerializableCrop
{
    // Tên của ScriptableObject (CropData) để biết đây là cây gì.
    // Ví dụ: "rice" (lúa), "berry" (dâu).
    public string cropName;

    // Vị trí của ô đất (Tile) mà cây này đang trồng.
    // Dùng Vector3Int vì Tilemap sử dụng tọa độ số nguyên (grid).
    public Vector3Int position;

    // Giai đoạn phát triển hiện tại của cây (ví dụ: 0=hạt giống, 1=nảy mầm, 2=trưởng thành).
    public int currentStage;

    // Thời gian còn lại (tính bằng giây) cho đến khi cây chuyển sang giai đoạn tiếp theo.
    public float timeRemaining;

    // Trạng thái của bộ đếm thời gian.
    // Thường dùng để kiểm tra xem cây đã được tưới nước hay chưa.
    // Nếu "true" (đã tưới), timeRemaining sẽ giảm; nếu "false" (chưa tưới), bộ đếm sẽ dừng.
    public bool timerIsRunning;
}

/// <summary>
/// Class phụ để lưu các vật phẩm "rơi" ra thế giới (Pickup items).
/// Ví dụ: Trứng gà, Sữa bò, Gỗ... nằm trên mặt đất chờ người chơi nhặt.
/// </summary>
[System.Serializable]
public class SerializablePickup
{
    // Tên (ID) của item, dùng để tra cứu ScriptableObject khi load.
    public string itemName;
    // Số lượng (nếu vật phẩm có thể "chồng" lên nhau khi rơi ra).
    public int count;
    // Vị trí chính xác trong thế giới game.
    public Vector3 position;
}

/// <summary>
/// Class phụ để lưu các vật thể "có thể thu hoạch" được (Harvestable).
/// Ví dụ: một bụi dâu dại, một mỏ quặng, hoặc một cây trưởng thành đã sẵn sàng
/// để spawn ra vật phẩm (thay vì tự chuyển thành vật phẩm).
/// </summary>
[System.Serializable]
public class SerializableHarvest
{
    // Tên của Prefab sẽ được spawn lại khi load game.
    public string prefabName;
    // Vị trí trong thế giới.
    public Vector3 position;
    // Hướng xoay (quan trọng nếu vật thể bị lật, nghiêng...).
    public Quaternion rotation;
}



// ---- "BÌNH CHỨA" DỮ LIỆU GAME CHÍNH ----

/// <summary>
/// Đây là class "cha" chứa TẤT CẢ dữ liệu của game cần được lưu lại.
/// Khi lưu game, ta sẽ tạo một đối tượng (instance) của class này,
/// điền dữ liệu vào các trường (field) của nó, và tuần tự hóa (serialize)
/// TOÀN BỘ đối tượng GameData này thành một file JSON duy nhất.
/// </summary>
[System.Serializable]
public class GameData
{
    // --- Dữ liệu Người chơi & Thế giới ---
    public long money; // Tiền của người chơi
    public int day; // Ngày hiện tại trong game
    public Vector3 playerPosition; // Vị trí cuối cùng của người chơi
    public float timeElapsed; // Thời gian đã trôi qua trong ngày (dùng để tính giờ trong game)
    public string runningEventName; // Tên của sự kiện (event/cutscene) đang diễn ra (nếu có)

    // --- Dữ liệu Inventory ---
    // Danh sách các ô chứa vật phẩm trong túi đồ của người chơi.
    public List<SerializableItemSlot> inventorySlots;

    // --- Dữ liệu Trang trại ---
    // Danh sách tất cả động vật trong trang trại.
    public List<SerializableAnimal> savedAnimals;
    // Danh sách tất cả cây trồng đang phát triển.
    public List<SerializableCrop> savedCrops;
    // Danh sách các vật phẩm đang rơi trên mặt đất.
    public List<SerializablePickup> savedPickups;
    // Danh sách các vật thể thu hoạch được.
    public List<SerializableHarvest> savedHarvests;

    // --- Dữ liệu Thế giới (Map) ---
    // Danh sách tọa độ (Vector3Int) của tất cả các ô đất đã được cuốc.
    // Dùng để load lại trạng thái "đã cuốc" của Tilemap.
    public List<Vector3Int> plowedTiles;

    // Danh sách ID của các đối tượng đã bị phá hủy vĩnh viễn.
    // Ví dụ: người chơi phá một tảng đá "độc nhất" (có ID "BigRock_01").
    // Ta lưu ID này lại để khi load game, tảng đá đó không xuất hiện lại nữa.
    public List<string> destroyedObjectIDs;

    /// <summary>
    /// Constructor (Hàm khởi tạo) của class GameData.
    /// Hàm này sẽ được gọi khi bạn tạo một "new GameData()".
    /// Mục đích chính của nó là ĐẶT GIÁ TRỊ MẶC ĐỊNH cho một file save MỚI.
    /// Nếu không có constructor, các giá trị (như money, day) sẽ là 0,
    /// và quan trọng nhất là các List<> sẽ bị "null".
    /// </summary>
    public GameData()
    {
        // Giá trị mặc định cho game mới
        this.money = 300;
        this.day = 0;
        this.playerPosition = new Vector3(0, 0, 0); // Nên đặt vị trí bắt đầu của game
        this.timeElapsed = 0f;
        this.runningEventName = ""; // Không có event gì khi bắt đầu

        // --- QUAN TRỌNG ---
        // Khởi tạo các List<> để chúng không bị "null".
        // Nếu không khởi tạo, bạn sẽ bị lỗi "NullReferenceException"
        // khi cố gắng .Add() bất cứ thứ gì vào các danh sách này.
        this.inventorySlots = new List<SerializableItemSlot>();
        this.savedAnimals = new List<SerializableAnimal>();
        this.savedCrops = new List<SerializableCrop>();
        this.savedPickups = new List<SerializablePickup>();
        this.plowedTiles = new List<Vector3Int>();
        this.destroyedObjectIDs = new List<string>();
        this.savedHarvests = new List<SerializableHarvest>(); // (Bạn đã thêm cái này, rất tốt!)
    }
}