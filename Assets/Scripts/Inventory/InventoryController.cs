using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject toolbarPanel;
    [SerializeField] GameObject sellPanel;
    public bool isOpen = false;

    private void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(sellPanel.activeInHierarchy)
            {
                return;
            }
            panel.SetActive(!panel.activeInHierarchy);
            toolbarPanel.SetActive(!toolbarPanel.activeInHierarchy);
            if (panel.activeInHierarchy)
                isOpen = true;
            else
                isOpen = false;
        }
    }
}
