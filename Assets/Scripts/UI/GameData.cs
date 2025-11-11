using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class phụ để lưu item (vì JSON không lưu được ScriptableObject)
[System.Serializable]
public class SerializableItemSlot
{
    public string itemName;
    public int count;
}

// Class phụ để lưu thú nuôi
[System.Serializable]
public class SerializableAnimal
{
    public string animalName; // "Cow" hoặc "Chicken"
    public Vector3 position;
    public float productionTimer;
}

// Class phụ để lưu cây trồng
[System.Serializable]
public class SerializableCrop
{
    public string cropName; // "rice", "berry", v.v.
    public Vector3Int position;
    public int currentStage;
    public float timeRemaining;
    public bool timerIsRunning; // Đã tưới nước hay chưa
}

// --- CLASS PHỤ MỚI ĐỂ LƯU VẬT PHẨM (Trứng, Sữa...) ---
[System.Serializable]
public class SerializablePickup
{
    public string itemName;
    public int count;
    public Vector3 position;
}


// ---- "BÌNH CHỨA" DỮ LIỆU GAME CHÍNH ----
[System.Serializable]
public class GameData
{
    // Dữ liệu cũ
    public long money;
    public int day;
    public Vector3 playerPosition;
    public List<SerializableItemSlot> inventorySlots;
    public float timeElapsed;
    public string runningEventName;
    public List<SerializableAnimal> savedAnimals;
    public List<SerializableCrop> savedCrops;
    public List<string> destroyedObjectIDs;
    // --- DỮ LIỆU MỚI ---
    public List<SerializablePickup> savedPickups; // Danh sách vật phẩm rơi ra
    public List<Vector3Int> plowedTiles; // Lưu các ô đã cuốc
    // Constructor - Đặt giá trị mặc định cho game MỚI
    public GameData()
    {
        this.money = 300;
        this.day = 0;
        this.playerPosition = new Vector3(0, 0, 0);
        this.inventorySlots = new List<SerializableItemSlot>();
        this.timeElapsed = 0f;
        this.runningEventName = "";
        this.savedAnimals = new List<SerializableAnimal>();
        this.savedCrops = new List<SerializableCrop>();

        // Khởi tạo list mới
        this.savedPickups = new List<SerializablePickup>();
        this.plowedTiles = new List<Vector3Int>(); // Khởi tạo list
        this.destroyedObjectIDs = new List<string>();
    }
}