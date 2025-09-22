using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropsReadController : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] List<CropData> cropDatas;
    Dictionary<TileBase, CropData> cropsFromTiles;

    private void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap in CropsReadController is not assigned!");
        }
        cropsFromTiles = new Dictionary<TileBase, CropData>();

        foreach (CropData cropData in cropDatas)
        {
            foreach (TileBase tile in cropData.tiles)
            {
                cropsFromTiles.Add(tile, cropData);
            }
        }
    }

    public Vector3Int GetGridPosition(Vector2 position, bool mousePosition)
    {
        Vector3 worldPosition;


        if (mousePosition)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(position);
        }
        else
        {
            worldPosition = position;
        }


        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);

        return gridPosition;
    }


    public TileBase GetTileBase(Vector3Int gridPosition)
    {
        TileBase tile = tilemap.GetTile(gridPosition);
        if (tile == null)
            Debug.LogWarning($"No tile at cell {gridPosition} on Tilemap {tilemap.name}");
        else
            Debug.Log($"Tile at {gridPosition} is {tile.name}");

        return tile;
    }

    public CropData GetCropData(TileBase tilebase)
    {
        return cropsFromTiles[tilebase];
    }
}


