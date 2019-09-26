using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] public static bool pauseOn;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject InGamePanel;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject InventoryPanel;
    Inventory inventoryScr;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
        InGamePanel = UICanvas.transform.GetChild(0).gameObject;
        PausePanel = UICanvas.transform.GetChild(1).gameObject;
        InventoryPanel = UICanvas.transform.GetChild(2).gameObject;
        inventoryScr = InventoryPanel.GetComponent<Inventory>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            pauseOn = !pauseOn;
            Pause();
        }
    }

    private void Pause()
    {
        if (pauseOn)
        {
            Debug.Log("Pause");
            Time.timeScale = 0;
            PausePanel.SetActive(true);
            inventoryScr.SlotInteractToggle();

        }
        else
        {
            Debug.Log("Resume");
            Time.timeScale = 1;
            PausePanel.SetActive(false);
            inventoryScr.SlotInteractToggle();
        }
    }
}