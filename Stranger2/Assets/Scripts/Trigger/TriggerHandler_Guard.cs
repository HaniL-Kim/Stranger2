using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler_Guard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerSound"))
        {
            FindObjectOfType<UIController>().GameOver();
        }
    }
}