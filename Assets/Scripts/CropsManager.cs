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
    [SerializeField] TileBase grass;
    [SerializeField] TileBase plowed;
    [SerializeField] TileBase toWater;
    [SerializeField] TileBase watered;
    [SerializeField] TileBase invisible;
    [Header("Tilemaps")]
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Tilemap cropTilemap;
    [Header("Runtime Parents")]
    [SerializeField] Transform harvestParent;
    Dictionary<Vector2Int, TileData> fields = new Dictionary<Vector2Int, TileData>();
    [Header("Crop Settings")]
    [SerializeField] List<CropInfo> cropList = new();

    Dictionary<string, CropInfo> cropInfos = new();
    Dictionary<Vector3Int, Crop> crops = new();
    private void Start()
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
                // Nếu có harvestPrefab => spawn instance để xử lý thu hoạch
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
        groundTilemap.SetTile(pos,  grass);
    }
}