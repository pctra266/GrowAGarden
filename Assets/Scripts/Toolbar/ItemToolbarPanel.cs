using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToolbarPanel : ItemPanel
{
    [SerializeField] ToolbarController toolbarController;
    int currentSelectedTool;

    private void Start()
    {
        Init();
        if (buttons != null && buttons.Count > 0)
        {
            Highlight(0);
        }
        else
        {
            Debug.LogWarning("No buttons found in ItemToolbarPanel!");
        }
    }

    // Choosing a new item in the toolbar
    public override void OnClick(int id)
    {
        toolbarController.Set(id);
        Highlight(id);

    }

    // Responsible for highlighting the button and hiding the previously selected highlight
    public void Highlight(int id)
    {
        buttons[currentSelectedTool].Highlight(false);
        currentSelectedTool = id;
        buttons[currentSelectedTool].Highlight(true);
    }
}
