using UnityEngine;
using UnityEngine.UI; // Cần thiết để làm việc với RawImage

public class ScrollBackground : MonoBehaviour
{
    // Kéo đối tượng RawImage (DynamicBackground) vào đây
    [SerializeField] private RawImage backgroundImage;

    [Header("Tốc độ cuộn")]
    [SerializeField] private float scrollSpeedX = 0.05f;
    [SerializeField] private float scrollSpeedY = 0.0f;

    private float offsetX = 0f;
    private float offsetY = 0f;

    void Update()
    {
        // Chúng ta dùng Time.unscaledDeltaTime để menu vẫn chuyển động
        // ngay cả khi game đang bị pause (Time.timeScale = 0)

        // Tính toán offset mới, và dùng toán tử % (modulo) để nó luôn lặp lại từ 0 đến 1
        offsetX = (offsetX + scrollSpeedX * Time.unscaledDeltaTime) % 1f;
        offsetY = (offsetY + scrollSpeedY * Time.unscaledDeltaTime) % 1f;

        // Áp dụng offset mới vào uvRect của RawImage
        // uvRect là một Rect(x, y, width, height)
        // Chúng ta chỉ thay đổi x và y, giữ nguyên width và height là 1
        backgroundImage.uvRect = new Rect(offsetX, offsetY, 1, 1);
    }
}