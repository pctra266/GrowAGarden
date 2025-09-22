using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_ShopController : MonoBehaviour
{
    private Transform container;
    private Transform shopItemTemplate;
    [SerializeField] private MoneyController money;
    private Button btn;
    [SerializeField] GameObject toolbarPanel;
    [SerializeField] GameObject inventoryPanel;
    public bool isOpen;

    private void Awake()
    {
        container = transform.Find("container");
        shopItemTemplate = container.Find("shopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false); 
    }

    private void Start()
    {

        Dictionary<string, Sprite> plantsDictionary = CreateSeedsFromSprite();
        CreateItemButton(plantsDictionary["Seeds_Berry"], "Seeds_Berry", 110, 0, "Berry");
        CreateItemButton(plantsDictionary["Seeds_Rice"], "Seeds_Rice", 100, 1, "Rice");

        gameObject.SetActive(false);
        Hide();
    }

    private Dictionary<string, Sprite> CreateSeedsFromSprite()
    {
        Dictionary<string, Sprite> plantsDictionary = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Plants");

        foreach (Sprite sprite in sprites)
        {
            plantsDictionary.Add(sprite.name, sprite);
        }

        return plantsDictionary;

    }

    private void CreateItemButton(Sprite itemSprite, string itemName, int itemCost, int positionIndex, string displayedName)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();
        float shopItemHeight = 120f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, 150 + (-shopItemHeight * positionIndex));
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
            FindFirstObjectByType<SoundManager>().Play("Money");
            Debug.Log("buy " + itemCost);
            if (item.Name.Contains("Seeds_Rice"))
            {
                GameManager.instance.inventoryContainer.Add(item, 4);
            }
            else if (item.Name.Contains("Seeds_Berry"))
            {
                GameManager.instance.inventoryContainer.Add(item, 4);
            }
        }


        toolbarPanel.SetActive(!toolbarPanel.activeInHierarchy);
        toolbarPanel.SetActive(true);
    }

    public void Show()
    {
        isOpen = true;
        gameObject.SetActive(true);

    }

    public void Hide()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }


    private void Update()
    {
        inventoryPanel.SetActive(false);
        toolbarPanel.SetActive(true);
    }
}
