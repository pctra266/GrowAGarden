using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarController : MonoBehaviour
{
    // We can later change it in the inspector because it's serialized

    //[SerializeField] int toolbarSize = 8;
    public int selectedTool;                               // Holds the id of the selected tool


    internal void Set(int id)
    {
        selectedTool = id;
    }

    public Item GetItem
    {
        get
        {
            if (GameManager.instance.inventoryContainer == null) return null;

            var slots = GameManager.instance.inventoryContainer.slots;
            if (selectedTool < 0 || selectedTool >= slots.Count)
                return null;

            return slots[selectedTool].item;
        }
    }

    public int GetCount
    {
        get
        {
            if (GameManager.instance.inventoryContainer == null) return 0;

            var slots = GameManager.instance.inventoryContainer.slots;
            if (selectedTool < 0 || selectedTool >= slots.Count)
                return 0;

            return slots[selectedTool].count;
        }
    }


}
