using UnityEngine;

// Script này chỉ dùng 1 lần để xóa bộ nhớ
public class _ResetPlayerPrefs : MonoBehaviour
{
    // Click chuột phải vào component này trong Inspector để chạy
    [ContextMenu("XÓA HẾT PLAYERPREFS")]
    void DeleteAllPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("ĐÃ XÓA HẾT PLAYERPREFS! HÃY TẮT GAME VÀ CHẠY LẠI.");
    }
}