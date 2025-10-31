using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_ShopController : MonoBehaviour
{
    [SerializeField]
    private Transform container;
    [SerializeField]
    private Transform shopItemTemplate;
    [SerializeField] private MoneyController money;
    private Button btn;
    [SerializeField] GameObject toolbarPanel;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField]
    private GameObject buyButton;

    [SerializeField] private AnimalItem cowItem;
    [SerializeField] private AnimalItem chickenItem;
    [SerializeField] private Transform animalSpawnPoint;
    
    public bool isOpen;

    private void Awake()
    {
       
        shopItemTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {

        Dictionary<string, Sprite> plantsDictionary = CreateSeedsFromSprite();
        CreateItemButton(plantsDictionary["Seeds_Berry"], "Seeds_Berry", 50, 0, "Berry");
        CreateItemButton(plantsDictionary["Seeds_Rice"], "Seeds_Rice", 20, 1, "Rice");
        CreateItemButton(plantsDictionary["Seeds_Tomato"], "Seeds_Tomato", 60, 2, "Tomato");
        CreateItemButton(plantsDictionary["Seeds_Pineapple"], "Seeds_Pineapple", 150, 3, "Pineapple");
        CreateItemButton(plantsDictionary["Seeds_Cabbage"], "Seeds_Cabbage", 400, 4, "cabbage");
        CreateItemButton(plantsDictionary["Seeds_Cloud"], "Seeds_Cloud", 1500, 5, "cloud");
        if (cowItem != null) CreateAnimalButton(cowItem, 6, "Bò Sữa");
        if (chickenItem != null) CreateAnimalButton(chickenItem, 7, "Gà");
        gameObject.SetActive(false);
        Hide();

    }

    private Dictionary<string, Sprite> CreateSeedsFromSprite()
    {
        Dictionary<string, Sprite> plantsDictionary = new Dictionary<string, Sprite>();

        List<Sprite> sprites = new List<Sprite>();
        sprites.AddRange(Resources.LoadAll<Sprite>("Plants"));
        sprites.AddRange(Resources.LoadAll<Sprite>("Plants2"));

        foreach (Sprite sprite in sprites)
        {
            if (!plantsDictionary.ContainsKey(sprite.name))
            {
                plantsDictionary.Add(sprite.name, sprite);
            }
            else
            {
                Debug.LogWarning($"Duplicate sprite name detected: {sprite.name}");
            }
        }

        return plantsDictionary;
    }


    private void CreateItemButton(Sprite itemSprite, string itemName, int itemCost, int positionIndex, string displayedName)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        shopItemTransform.SetParent(container, false);
        shopItemTransform.gameObject.SetActive(true);
        shopItemTransform.Find("nameText").GetComponent<TextMeshProUGUI>().SetText(displayedName);
        shopItemTransform.Find("priceText").GetComponent<TextMeshProUGUI>().SetText(itemCost.ToString());
        shopItemTransform.Find("itemIcon").GetComponent<Image>().sprite = itemSprite;

        Item newItem = ScriptableObject.CreateInstance<Item>();

        foreach (ItemSlot itemSlot in GameManager.instance.allItemsContainer.slots)
        {
            if (itemSlot.item.Name == itemName)
            {
                newItem = itemSlot.item;
            }
        }

        btn = shopItemTransform.GetComponent<Button>();
        btn.onClick.AddListener(delegate { TaskWithParameters(itemCost, newItem); });
    }

    void TaskWithParameters(long itemCost, Item item)
    {
        if (money.canBuyItems(itemCost))
        {
            money.substractMoney(itemCost);
            //FindFirstObjectByType<SoundManager>().Play("Money");
            Debug.Log("buy " + itemCost);
            if (item.Name.Contains("Seeds_Rice"))
            {
                GameManager.instance.inventoryContainer.Add(item, 1);
            }
            else if (item.Name.Contains("Seeds_Berry"))
            {
                GameManager.instance.inventoryContainer.Add(item, 10);
            }
            else if (item.Name.Contains("Seeds_Tomato"))
            {
                GameManager.instance.inventoryContainer.Add(item, 1);
            }
            else if (item.Name.Contains("Seeds_Pineapple"))
            {
                GameManager.instance.inventoryContainer.Add(item, 1);
            }
            else if (item.Name.Contains("Seeds_Cabbage"))
            {
                GameManager.instance.inventoryContainer.Add(item, 1);
            }
            else if (item.Name.Contains("Seeds_Cloud"))
            {
                GameManager.instance.inventoryContainer.Add(item, 1);
            }
        }


        toolbarPanel.SetActive(!toolbarPanel.activeInHierarchy);
        toolbarPanel.SetActive(true);
    }

    public void Show()
    {
        Time.timeScale = 0f;
        buyButton.SetActive(false);
        isOpen = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        isOpen = false;
        gameObject.SetActive(false);
        buyButton.SetActive(true);
    }


    private void Update()
    {
        inventoryPanel.SetActive(false);
        toolbarPanel.SetActive(true);
    }


    private void CreateAnimalButton(AnimalItem animalData, int positionIndex, string displayedName)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();
        shopItemTransform.Find("nameText").GetComponent<TextMeshProUGUI>().SetText(displayedName);
        shopItemTransform.Find("priceText").GetComponent<TextMeshProUGUI>().SetText(animalData.purchaseCost.ToString());
        shopItemTransform.Find("itemIcon").GetComponent<Image>().sprite = animalData.icon;

        btn = shopItemTransform.GetComponent<Button>();
        btn.onClick.AddListener(() => BuyAnimal(animalData));
    }

    void BuyAnimal(AnimalItem animalToBuy)
    {
        if (money.canBuyItems(animalToBuy.purchaseCost))
        {
            money.substractMoney(animalToBuy.purchaseCost);
            //FindFirstObjectByType<SoundManager>()?.Play("Money");
            AnimalManager.Instance.PurchaseAnimal(animalToBuy, animalSpawnPoint.position);
        }
        else
        {
            Debug.Log("Không đủ tiền!");
        }
    }
}
