using UnityEngine;

public class RainTriggerEvent : MonoBehaviour, ICompletableEvent
{
    [SerializeField] private GameObject rainEffectPrefab;
    [SerializeField] private Transform spawnPoint; // vị trí tạo hiệu ứng mưa
    [SerializeField] private float duration = 10f;

    public event System.Action OnCompleted;

    public void TriggerEvent()
    {
        StartCoroutine(RainRoutine());
    }

    private System.Collections.IEnumerator RainRoutine()
    {
        // Nếu không gán spawnPoint, dùng vị trí của chính event
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;

        GameObject rainInstance = Instantiate(rainEffectPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"Rain started at {spawnPosition}");

        yield return new WaitForSeconds(duration);

        if (rainInstance != null)
        {
            Destroy(rainInstance);
            Debug.Log("Rain stopped!");
        }

        OnCompleted?.Invoke();
        Destroy(gameObject);
    }
}
