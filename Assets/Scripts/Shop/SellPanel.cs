using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SellPanel : ItemPanel
{
    [SerializeField] MoneyController moneyController;
    [SerializeField] Button sellButton;
    [SerializeField] Button closeButton;

    [Header("Inventory Reference")]
    [SerializeField] GameObject inventoryPanelObject;
    [Header("World Button Reference")]
    [SerializeField] GameObject sellTriggerButton;
    [SerializeField] GameObject toolBarPanel;
    [SerializeField] TextMeshProUGUI totalPriceText;
    [SerializeField] private InventoryController inventoryController;
    private void Start()
    {
        sellButton.onClick.AddListener(SellItems);
        closeButton.onClick.AddListener(Hide);
        inventory.Clear();
        Hide();
    }

    public override void OnClick(int id)
    {
        GameManager.instance.dragAndDropController.OnClick(inventory.slots[id]);
        ShowSellPanel();

    }

    public void SellItems()
    {
        long totalSellValue = 0;

        foreach (ItemSlot slot in inventory.slots)
        {
            if (slot.item != null)
            {
                totalSellValue += slot.item.sellPrice * slot.count;
            }
        }

        if (totalSellValue > 0)
        {
            moneyController.addMoney(totalSellValue);
            SoundManager.instance.Play("Money");
            inventory.Clear();
            ShowSellPanel();
        }
    }
    public void ShowSellPanel()
    {
        if (inventoryController != null && inventoryController.isOpen)
        {
            return;
        }
        Time.timeScale = 0f;
        gameObject.SetActive(true);
        inventoryPanelObject.SetActive(true);
        toolBarPanel.SetActive(false);
        base.Show();
        UpdateTotalPrice();
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        inventoryPanelObject.SetActive(false);
        toolBarPanel.SetActive(true);
        if (sellTriggerButton != null)
        {
            sellTriggerButton.SetActive(true);
        }
    }
    private long CalculateTotal()
    {
        long total = 0;
        foreach (ItemSlot slot in inventory.slots)
        {
            if (slot != null && slot.item != null)
            {
                total += (long)slot.item.sellPrice * slot.count;
            }
        }
        return total;
    }

    private void UpdateTotalPrice()
    {
        if (totalPriceText != null)
        {
            totalPriceText.text = CalculateTotal().ToString();
        }
    }
}