using System;
using UnityEngine;

public class NothingHappen : MonoBehaviour, ICompletableEvent
{
    public event Action OnCompleted;
    [SerializeField] private float duration = 30f;
    public void TriggerEvent()
    {
        StartCoroutine(NothingRoutine());
    }

    private System.Collections.IEnumerator NothingRoutine()
    {
        Debug.Log($"NothingHappen started — waiting {duration} seconds...");

        yield return new WaitForSeconds(duration);

        Debug.Log("NothingHappen finished.");
        OnCompleted?.Invoke();  
        Destroy(gameObject);    
    }
}
