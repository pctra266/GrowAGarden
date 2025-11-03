using UnityEngine;
using UnityEngine.UI;

public class SellTriggerController : MonoBehaviour
{
    [SerializeField] private SellPanel uiSellPanel;

    [SerializeField] private RectTransform sellButtonRectTransform;
    [SerializeField] private GameObject sellButtonPosition;
    [SerializeField] private InventoryController inventoryController;

    void Start()
    {
        if (sellButtonRectTransform != null)
        {
            sellButtonRectTransform.gameObject.SetActive(false);
        }

        Button sellButton = sellButtonRectTransform.GetComponent<Button>();
        sellButton.onClick.AddListener(OpenSellPanel);
    }

    private void OpenSellPanel()
    {
        if (inventoryController != null && inventoryController.isOpen)
        {
            return;
        }
        uiSellPanel.ShowSellPanel();
        sellButtonRectTransform.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            sellButtonRectTransform.gameObject.SetActive(true);
            Vector3 worldPosition = sellButtonPosition.transform.position;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
            sellButtonRectTransform.position = screenPoint;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (Time.timeScale == 0f)
        {
            return;
        }
        if (collider.gameObject.tag == "Player")
        {
            sellButtonRectTransform.gameObject.SetActive(false);
        }
    }
}