using UnityEngine;

public class ShopTriggerController : MonoBehaviour
{
    [SerializeField] private UI_ShopController uiShop;

    [SerializeField] private RectTransform buyButtonRectTransform;
    [SerializeField] private GameObject buyButtonPosition;

    void Start() { 
        if (buyButtonRectTransform != null)
        {
            buyButtonRectTransform.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            buyButtonRectTransform.gameObject.SetActive(true);
            Vector3 worldPosition = buyButtonPosition.transform.position;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
            buyButtonRectTransform.position = screenPoint;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            buyButtonRectTransform.gameObject.SetActive(false);
        }
    }
}
