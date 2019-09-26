using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerSound"))
        {
            Debug.Log("거... 거기 누구세요?");
        }
    }
}