using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class GameEvent
{
    public string eventName;
    [Range(0f, 1f)] public float chance = 1f;
    public float delay = 0f;
    public GameObject eventPrefab;

    [HideInInspector] public bool isRunning = false; 

    public void Trigger(MonoBehaviour caller)
    {
        caller.StartCoroutine(TriggerWithDelay(caller));
    }

    private IEnumerator TriggerWithDelay(MonoBehaviour caller)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        if (eventPrefab == null)
        {
            Debug.LogWarning($"[EventManager] Event prefab for '{eventName}' is missing!");
            yield break;
        }

        GameObject obj = GameObject.Instantiate(eventPrefab);
        IScriptTrigger trigger = obj.GetComponent<IScriptTrigger>();

        if (trigger != null)
        {
            isRunning = true;
            EventManager.instance.NotifyEventStarted(this);

            // nếu event script hỗ trợ "hoàn thành"
            if (trigger is ICompletableEvent completable)
            {
                completable.OnCompleted += () =>
                {
                    isRunning = false;
                    EventManager.instance.NotifyEventEnded(this);
                };
            }

            trigger.TriggerEvent();
        }
        else
        {
            Debug.LogWarning($"[EventManager] Prefab '{eventPrefab.name}' does not implement IScriptTrigger.");
        }
    }
}
public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    [SerializeField] private List<GameEvent> gameEvents = new List<GameEvent>();
    [Header("Auto Trigger Settings")]
    [Tooltip("Khoảng thời gian giữa các event (giây)")]
    public float eventInterval = 10f;

    [Tooltip("Tự động kích hoạt event ngẫu nhiên sau mỗi khoảng thời gian")]
    public bool autoTriggerEnabled = true;
    private Coroutine autoTriggerCoroutine;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (autoTriggerEnabled)
        {
            autoTriggerCoroutine = StartCoroutine(AutoTriggerRoutine());
        }
    }
    private IEnumerator AutoTriggerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(eventInterval);

            // chỉ trigger nếu có event và không event nào đang chạy
            bool anyRunning = gameEvents.Exists(e => e.isRunning);
            if (!anyRunning)
            {
                TriggerRandomEvent();
            }
        }
    }

    public void NotifyEventStarted(GameEvent e)
    {
        Debug.Log($"[EventManager] Event '{e.eventName}' started!");
    }

    public void NotifyEventEnded(GameEvent e)
    {
        Debug.Log($"[EventManager] Event '{e.eventName}' ended!");
    }

    public bool IsEventRunning(string eventName)
    {
        var e = gameEvents.Find(x => x.eventName == eventName);
        return e != null && e.isRunning;
    }

    public void TriggerEventByName(string eventName)
    {
        foreach (var e in gameEvents)
        {
            if (e.eventName == eventName)
            {
                e.Trigger(this);
                break;
            }
        }
    }

    public void TriggerRandomEvent()
    {
        if (gameEvents.Count == 0) return;

        float totalChance = 0f;
        foreach (var e in gameEvents) totalChance += e.chance;

        float roll = Random.value * totalChance;
        float cumulative = 0f;

        foreach (var e in gameEvents)
        {
            cumulative += e.chance;
            if (roll <= cumulative)
            {
                e.Trigger(this);
                break;
            }
        }
    }
}

public interface ICompletableEvent : IScriptTrigger
{
    event System.Action OnCompleted;
}
public interface IScriptTrigger
{
    void TriggerEvent();
}
