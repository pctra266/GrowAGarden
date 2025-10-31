using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class ToolsCharacterController : MonoBehaviour
{
    PlayerControl character;
    Rigidbody2D rgbd2d;
    [SerializeField] MarkerManager markerManager;
    [SerializeField] TileMapReadController tileMapReadController;
    [SerializeField] CropsReadController cropsReadController;
    [SerializeField] float maxDistance = 2f;
    [SerializeField] CropsManager cropsManager;
    [SerializeField] TileData plowableTiles;
    [SerializeField] TileData toMowTiles;
    [SerializeField] TileData toSeedTiles;
    [SerializeField] TileData waterableTiles;
    InventoryController inventoryController;
    ToolbarController toolbarController;
    [SerializeField] GameObject toolbarPanel;
    [SerializeField] private GameObject shopUIButton;

    [SerializeField] float offsetDistance = 1f;
    [SerializeField] float sizeOfInteractableArea = 1.2f;

    Vector3Int selectedTilePosition;
    bool selectable;

    public static Dictionary<Vector2Int, TileData> fields;
    public static Dictionary<Vector2Int, CropData> crops;

    UI_ShopController shopPanel;


    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<PlayerControl>();
        rgbd2d = GetComponent<Rigidbody2D>();
        fields = new Dictionary<Vector2Int, TileData>();
        crops = new Dictionary<Vector2Int, CropData>();
        toolbarController = GetComponent<ToolbarController>();
        inventoryController = GetComponent<InventoryController>();

        var shopPanelAll = Resources.FindObjectsOfTypeAll<UI_ShopController>();
        shopPanel = shopPanelAll[0];
    }

    void Update()
    {
        SelectTile();
        CanSelectCheck();
        Marker();
        if (Input.GetMouseButtonDown(0)) 
        {
            if (!inventoryController.isOpen) //you can use tools only if inventory is closed
            {
                if (UseToolWorld() == true)
                {
                    Debug.Log("Used tool on world object");
                    return;
                }
                UseTool();
            }

        }
    }

    private bool CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
           
            if (hit.collider.gameObject.tag.Equals("Destroyable"))
            {
                return true;
            }
        }
        return false;
    }
    private bool CastRayPlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            if (hit.collider.gameObject.name.Contains("Player"))
            {
                return true;
            }
        }
        return false;
    }
    private void SelectTile()
    {

        selectedTilePosition = tileMapReadController.GetGridPosition(Input.mousePosition, true);
        TileBase tileBase = tileMapReadController.GetTileBase(selectedTilePosition);
        try
        {
            TileData tileData = tileMapReadController.GetTileData(tileBase);
            if (!(tileData is null))
            {
                if (!fields.ContainsKey((Vector2Int)selectedTilePosition))
                {
                    fields.Add((Vector2Int)selectedTilePosition, tileData);
                }
                else
                {
                    fields[(Vector2Int)selectedTilePosition] = tileData;
                }
            }
        }
        catch
        {
            return;
        }
        TileBase cropBase = cropsReadController.GetTileBase(selectedTilePosition);
        try
        {
            CropData cropData = cropsReadController.GetCropData(cropBase);
            if (!(cropData is null))
            {
                if (!crops.ContainsKey((Vector2Int)selectedTilePosition))
                {
                    crops.Add((Vector2Int)selectedTilePosition, cropData);
                }
                else
                {
                    crops[(Vector2Int)selectedTilePosition] = cropData;
                }
            }
        }
        catch
        {
            return;
        }

    }

    void CanSelectCheck()
    {
        if (Time.timeScale == 0) //if game paused
            return;

        Vector2 characterPosition = transform.position;
        Vector2 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectable = Vector2.Distance(characterPosition, cameraPosition) < maxDistance;
        markerManager.Show(selectable);
    }

    private void Marker()
    {
        markerManager.markedCellPosition = selectedTilePosition;
    }

    // interacting with physical objects in the world
    private bool UseToolWorld()
    {
        if (shopUIButton != null && shopUIButton.activeInHierarchy)
        {
            return true; 
        }
        if (Time.timeScale == 0)
            return false;

        Vector2 position = rgbd2d.position + character.lastMotionVector * offsetDistance;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, sizeOfInteractableArea);

        foreach (Collider2D collidor in colliders)
        {
            PlayerHit hitPlayer = collidor.GetComponent<PlayerHit>();
            DeytroyableHit[] hitDestroyable = collidor.GetComponents<DeytroyableHit>();

            if(hitDestroyable != null &&  CastRay() == true)
            {
                foreach (DeytroyableHit destroyable in hitDestroyable)
                {
                        destroyable.Hit();
                        return true;
                }
            }
        }

        return false;
    }

    private void RefreshToolbar()
    {
        toolbarPanel.SetActive(!toolbarPanel.activeInHierarchy);
        toolbarPanel.SetActive(true);
    }

    private void UseTool()
    {
        if (Time.timeScale == 0) //if game paused - return
            return;

        // when sth is present on the grid but you can't plant there
        if (selectable == true && toolbarController.GetItem != null)
        {
            TileBase tileBase = tileMapReadController.GetTileBase(selectedTilePosition);
            TileData tileData = tileMapReadController.GetTileData(tileBase);
            if (tileData != plowableTiles && tileData != toMowTiles && tileData != toSeedTiles && tileData != waterableTiles) //if tile doesn't have any ability
            {
                return;
            }
     
            if (crops[(Vector2Int)selectedTilePosition].noPlant)
            {
                if (fields[(Vector2Int)selectedTilePosition].plowable && toolbarController.GetItem.Name == "Hoe")
                {
                    Debug.Log("Plow");
                    cropsManager.Plow(selectedTilePosition);
                }
                else if (fields[(Vector2Int)selectedTilePosition].ableToSeed && toolbarController.GetItem.isSeed == true)
                {
                    switch (toolbarController.GetItem.Name) //depending on what seed you have chosen
                    {
                        case "Seeds_Rice":
                                cropsManager.SeedCrop(selectedTilePosition, "rice");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, 1); 
                            break;
                        case "Seeds_Berry":
                                cropsManager.SeedCrop(selectedTilePosition, "berry");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, 1); 
                            break;
                        case "Seeds_Pineapple":
                                cropsManager.SeedCrop(selectedTilePosition, "pineapple");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, 1); 
                            break;
                        case "Seeds_Cabbage":
                                cropsManager.SeedCrop(selectedTilePosition, "cabbage");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, 1); 
                            break;
                        case "Seeds_Tomato":
                                cropsManager.SeedCrop(selectedTilePosition, "tomato");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, 1); 
                            break;
                        case "Seeds_Cloud":
                                cropsManager.SeedCrop(selectedTilePosition, "cloud");
                                GameManager.instance.inventoryContainer.RemoveItem(toolbarController.GetItem, 1); 
                            break;
                    }

                    RefreshToolbar();
                }
            }
            else if (crops[(Vector2Int)selectedTilePosition].planted && fields[(Vector2Int)selectedTilePosition].waterable && toolbarController.GetItem.Name == "WateringCan")
            {
                cropsManager.Water(selectedTilePosition);
                FindFirstObjectByType<SoundManager>().Play("Water");
            }
        }
    }
}
