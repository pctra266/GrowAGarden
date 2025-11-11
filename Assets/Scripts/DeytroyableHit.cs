using System.Collections.Generic;
using UnityEngine;

public class DeytroyableHit : ToolHit
{
    [Tooltip("ĐẶT MỘT ID DUY NHẤT CHO VẬT NÀY (vd: StartingChest_01)")]
    public string uniqueID; // <-- BIẾN MỚI QUAN TRỌNG ĐÂY!

    [SerializeField] GameObject[] pickUpItem;
    [SerializeField] int dropCount = 2;
    [SerializeField] float spread = 0.9f;

    List<GameObject> items;

    private void Start()
    {

        if (!string.IsNullOrEmpty(uniqueID) && MainMenuManager.IsLoadingGame)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.IsObjectDestroyed(uniqueID))
            {
                Destroy(gameObject);
                return;
            }
        }

        items = new List<GameObject>();
        if (pickUpItem.Length == 0)
        {
            Debug.LogWarning("No items assigned to destroyable object");
        }
        else
        {
            for (int i = 0; i < pickUpItem.Length; i++)
            {
                items.Add(pickUpItem[i]);
            }
        }
    }

    public override void Hit()
    {

        if (!string.IsNullOrEmpty(uniqueID) && SaveManager.Instance != null)
        {
            SaveManager.Instance.RecordDestroyedObject(uniqueID);
        }

        for (int i = 0; i < dropCount && i < items.Count; i++)
        {
            Vector3 position = transform.position;
            position.x -= spread * UnityEngine.Random.value - spread / 2;
            position.y -= spread * UnityEngine.Random.value - spread / 2;

            GameObject newObject = Instantiate(items[i]);
            newObject.transform.position = position;
        }
        SoundManager.instance.Play("DestroyObject");
        Destroy(gameObject);
    }
}