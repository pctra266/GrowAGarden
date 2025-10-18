using System.Collections.Generic;
using UnityEngine;

public class DeytroyableHit : ToolHit
{
    [SerializeField] GameObject[] pickUpItem;
    [SerializeField] int dropCount = 2;
    [SerializeField] float spread = 0.9f;

    List<GameObject> items;
    private void Start()
    {
        items = new List<GameObject>();
        if(pickUpItem.Length == 0)
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
        for (int i = 0; i < dropCount && i < items.Count; i++)
        {
            Vector3 position = transform.position;
            position.x -= spread * UnityEngine.Random.value - spread / 2;
            position.y -= spread * UnityEngine.Random.value - spread / 2;

            GameObject newObject = Instantiate(items[i]);
            newObject.transform.position = position;
        }
        Destroy(gameObject);

    }
}
