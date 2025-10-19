using UnityEngine;

public class Product : MonoBehaviour
{
    // Cần một cách để tham chiếu đến Item Data, ví dụ dùng ScriptableObject
    // public ItemData itemData; 

    void OnMouseDown()
    {
        Debug.Log("Đã thu thập " + gameObject.name);

        // Tìm đối tượng quản lý inventory (giả sử có một đối tượng với tag "InventoryManager")
        InventoryController inventoryManager = FindObjectOfType<InventoryController>();

        if (inventoryManager != null)
        {
            // Gọi hàm AddItem trong InventoryManager
            // inventoryManager.AddItem(itemData);

            // Sau khi thêm vào kho, hủy đối tượng sản phẩm
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("Không tìm thấy InventoryManager!");
        }
    }
}
