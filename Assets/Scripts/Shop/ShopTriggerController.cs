using UnityEngine;

public class ShopTriggerController : MonoBehaviour
{
    [SerializeField] private UI_ShopController uiShop;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
        uiShop.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            uiShop.Hide();
        }
    }
}
