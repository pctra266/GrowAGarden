using UnityEngine;

public class ShopTriggerController : MonoBehaviour
{
    [SerializeField] private UI_ShopController uiShop;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        uiShop.Show();
        FindFirstObjectByType<SoundManager>().Play("Money");
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        uiShop.Hide();
    }
}
