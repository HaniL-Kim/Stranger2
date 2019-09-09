using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public GameObject pausePanel;
    [SerializeField] public Item itemContainerOrigin;
    [SerializeField] public Item itemContainerDestin;
    [SerializeField] public Image selectedIcon;
    [SerializeField] public bool isSelect = false;
    public bool isInteractable;

    private void Awake()
    {
        pausePanel = this.transform.parent.transform.GetChild(1).gameObject;
        selectedIcon = this.transform.GetChild(9).GetComponent<Image>();
    }

    private void Update()
    {
        if(isSelect)
        {
            selectedIcon.transform.position = Input.mousePosition;
        }
    }

    public void SlotInteractToggle()
    {
        isInteractable = pausePanel.activeSelf ? true : false;
        for (int i = 0; i < this.transform.childCount-1; i++)
        {
            this.transform.GetChild(i).GetComponent<Button>().interactable = isInteractable;
        }
    }
}
