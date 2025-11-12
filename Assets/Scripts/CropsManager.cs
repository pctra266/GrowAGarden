using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class CropInfo
{
    public string name;
    public Crop template;
    public float growTime;
    public int maxStage;
    public GameObject harvestPrefab;
}
public class CropsManager : MonoBehaviour
{
    [Header("Ground Tiles")]
    [SerializeField] public TileBase grass;
    [SerializeField] public TileBase plowed;
    [SerializeField] public TileBase mowed;
    [SerializeField] public TileBase toWater;
    [SerializeField] public TileBase watered;
    [SerializeField] public TileBase invisible;
    [Header("Tilemaps")]
    [SerializeField] public Tilemap groundTilemap;
    [SerializeField] public Tilemap cropTilemap;


    [Header("Runtime Parents")]
    [SerializeField] public Transform harvestParent;
    Dictionary<Vector2Int, TileData> fields = new Dictionary<Vector2Int, TileData>();
    [Header("Crop Settings")]
    [SerializeField] List<CropInfo> cropList = new();

    Dictionary<string, CropInfo> cropInfos = new();

    // --- THAY ĐỔI QUAN TRỌNG: BIẾN NÓ THÀNH PUBLIC ---
    public Dictionary<Vector3Int, Crop> crops = new();

    private void Awake()
    {
        foreach (var info in cropList)
            cropInfos[info.name.ToLower()] = info;
    }

    private void Update()
    {
        rainAutoWater();
        var cropListCopy = new List<Crop>(crops.Values);

        foreach (var crop in cropListCopy)
            Grow(crop);
    }

    public void rainAutoWater()
    {
        if (EventManager.instance != null && EventManager.instance.IsEventRunning("Rain"))
        {
            List<Vector3Int> positions = new List<Vector3Int>(crops.Keys);
            foreach (var pos in positions)
            {
                Water(pos);
            }
        }
    }
    // --- Ground actions ---
    public void Plow(Vector3Int pos) => groundTilemap.SetTile(pos, plowed);

    // --- Seed a crop ---
    public void SeedCrop(Vector3Int pos, string name)
    {
        name = name.ToLower();
        if (!cropInfos.TryGetValue(name, out var info)) return;

        var crop = Instantiate(info.template);
        crop.name = name;
        crop.position = pos;
        crop.state = crop.state0;
        crop.timeRemaining = info.growTime;

        crops[pos] = crop;

        cropTilemap.SetTile(pos, crop.state0);
        groundTilemap.SetTile(pos, toWater);
    }

    // --- Water a crop ---
    public void Water(Vector3Int pos)
    {
        if (!crops.TryGetValue(pos, out var crop)) return;
        crop.timerIsRunning = true;
        groundTilemap.SetTile(pos, watered);
    }

    // --- Grow logic ---
    void Grow(Crop crop)
    {
        if (!crop.timerIsRunning) return;
        float delta = Time.deltaTime;
        if (EventManager.instance != null && EventManager.instance.IsEventRunning("Rain"))
        {
            delta *= 2f;
        }
        crop.timeRemaining -= delta;
        if (crop.timeRemaining > 0) return;

        crop.NextState();
        cropTilemap.SetTile(crop.position, crop.state);

        if (cropInfos.TryGetValue(crop.name, out var info))
        {
            if (crop.currentStage < info.maxStage)
            {
                crop.timeRemaining = info.growTime;
            }
            else
            {
                // === Final stage reached ===
                if (info.harvestPrefab != null)
                {
                    Vector3 worldPos = cropTilemap.GetCellCenterWorld(crop.position);
                    var go = Instantiate(info.harvestPrefab, worldPos, Quaternion.identity, harvestParent);
                    cropTilemap.SetTile(crop.position, invisible);

                    Collect(crop.position);

                }
                else
                {
                    crop.timerIsRunning = false;
                }
            }
        }

    }

    // --- Harvest ---
    public void Collect(Vector3Int pos)
    {
        if (!crops.TryGetValue(pos, out var crop)) return;

        cropTilemap.SetTile(pos, invisible);
        Destroy(crop);
        crops.Remove(pos);

        // MoneyController.AddGold(20);
        groundTilemap.SetTile(pos, grass);
    }

    // --- HÀM MỚI DÀNH CHO SAVE/LOAD ---

    // Hàm này được gọi bởi SaveManager để xóa sạch cây trồng cũ trước khi tải
    public void ClearAllCrops()
    {
        List<Vector3Int> positions = new List<Vector3Int>(crops.Keys);
        foreach (var pos in positions)
        {
            cropTilemap.SetTile(pos, null);
            groundTilemap.SetTile(pos, grass); // Trả về cỏ
            if (crops.ContainsKey(pos) && crops[pos] != null)
            {
                Destroy(crops[pos]); // Phá hủy ScriptableObject instance
            }
        }
        crops.Clear(); // Xóa sạch dictionary
    }

    // Hàm này được gọi bởi SaveManager để tái tạo cây trồng từ file save
    public void LoadCrop(SerializableCrop data)
    {
        // 1. Tìm thông tin (template) của cây trồng
        if (!cropInfos.TryGetValue(data.cropName.ToLower(), out var info)) return;

        // 2. Tạo một bản sao ScriptableObject
        var crop = Instantiate(info.template);
        crop.name = data.cropName;
        crop.position = data.position;
        crop.currentStage = data.currentStage;
        crop.timeRemaining = data.timeRemaining;
        crop.timerIsRunning = data.timerIsRunning;

        // 3. Đặt lại Tile (hình ảnh) cho đúng
        switch (crop.currentStage)
        {
            case 0: crop.state = crop.state0; break;
            case 1: crop.state = crop.state1; break;
            case 2: crop.state = crop.state2; break;
            case 3: crop.state = crop.state3; break;
            case 4: crop.state = crop.state4; break;
            case 5: crop.state = crop.state5; break;
            default: crop.state = crop.state0; break;
        }

        // 4. Thêm lại vào Dictionary
        crops[data.position] = crop;

        // 5. Cập nhật hình ảnh trên Tilemap
        cropTilemap.SetTile(data.position, crop.state);
        // Cập nhật đất (tưới rồi hay chưa)
        groundTilemap.SetTile(data.position, data.timerIsRunning ? watered : toWater);
    }
    public CropInfo GetCropInfoByHarvestName(string harvestName)
    {
        foreach (var info in cropList)
        {
            if (info.harvestPrefab != null &&
                info.harvestPrefab.name.Replace("(Clone)", "").Trim().Equals(harvestName, System.StringComparison.OrdinalIgnoreCase))
            {
                return info;
            }
        }
        return null;
    }

}