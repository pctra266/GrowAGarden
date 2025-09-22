using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ChestHit : ToolHit
{
    [SerializeField] GameObject pickUpWateringCan;
    [SerializeField] GameObject pickUpHoe;

    [SerializeField] int dropCount = 2;
    [SerializeField] float spread = 0.9f;

    List<GameObject> items;

    private void Start()
    {
        items = new List<GameObject>();
        items.Add(pickUpWateringCan);
        items.Add(pickUpHoe);
    }

    public override void Hit()
    {
        // spawning objects
        while (dropCount > 0)
        {
            dropCount -= 1;

            // calculating where items will drop
            Vector3 position = transform.position;
            position.x -= spread * UnityEngine.Random.value - spread / 2;
            position.y -= spread * UnityEngine.Random.value - spread / 2;

            GameObject newObject = Instantiate(items[dropCount]);
            newObject.transform.position = position;
        }

        Destroy(gameObject);

    }
}
