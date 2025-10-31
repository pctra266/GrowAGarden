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

    private void Start()
    {

        sellButton.onClick.AddListener(SellItems);
        closeButton.onClick.AddListener(Hide);

        Hide();
    }

    public override void OnClick(int id)
    {
        GameManager.instance.dragAndDropController.OnClick(inventory.slots[id]);
        Show();
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
            inventory.Clear();
            Show();
        }
    }

    public void Show()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);
        inventoryPanelObject.SetActive(true);
        base.Show();
    }

    public void Hide()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        inventoryPanelObject.SetActive(false);
        if (sellTriggerButton != null)
        {
            sellTriggerButton.SetActive(true);
        }
    }
}