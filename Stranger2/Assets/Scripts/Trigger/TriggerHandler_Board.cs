using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler_Board : MonoBehaviour
{
    UIController uiController;
    PlayerRenderer playerRenderer;
    Vector3 boardDir;
    public bool isPlayerIn;
    public bool isReading;
    public string[] keysInBoard = new string[] { "Board0_key0", "Board0_key0", "Board0_key0", "Board0_key0" };
    public string[] textInBoard;
    public int keyCounter = 0;

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>();
        playerRenderer = FindObjectOfType<PlayerRenderer>();
        boardDir = new Vector3(-1, 1, 0); // NW Dir
        textInBoard = new string[keysInBoard.Length];
        StartCoroutine("GetLocalizedTextOfBoard");
    }

    private void Update()
    {
        if (isPlayerIn)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (!isReading)
                {
                    if (playerRenderer.playerDirection == boardDir)
                    {
                        uiController.StartCoroutine("StartReadBoard", this.gameObject.GetComponent<TriggerHandler_Board>());
                        keyCounter++;
                    }
                }
                else if (isReading)
                {
                    if (0 < keyCounter && keyCounter < 4)
                    {
                        uiController.StartCoroutine("ContinueReadBoard", this.gameObject.GetComponent<TriggerHandler_Board>());
                        keyCounter++;
                    }
                    else if (keyCounter >= 4)
                    {
                        uiController.StartCoroutine("EndReadBoard", this.gameObject.GetComponent<TriggerHandler_Board>());
                        keyCounter = 0;
                    }
                }
            }
        }
    }

    IEnumerator GetLocalizedTextOfBoard()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < keysInBoard.Length; i++)
        {
            textInBoard[i] = LocalizationManager.instance.GetLocalizedValue(keysInBoard[i]);
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerTrigger"))
        {
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("PlayerTrigger"))
        {
            keyCounter = 0;
            isPlayerIn = false;
            uiController.StartCoroutine("EndReadBoard", this.gameObject.GetComponent<TriggerHandler_Board>());
        }
    }
}