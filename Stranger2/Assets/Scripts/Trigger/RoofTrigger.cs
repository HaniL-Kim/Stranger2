using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofTrigger : MonoBehaviour
{
    private WareHouseTriggerHandler triggerHandler;

    public float fadeTime;
    float start, end;

    private void Awake()
    {
        triggerHandler = this.transform.parent.GetComponent<WareHouseTriggerHandler>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerTrigger"))
        {
            for (int i = 0; i < triggerHandler.SR_WareHouse_Roof_Arr.Length; i++)
            {
                StartCoroutine("FadeOut", triggerHandler.SR_WareHouse_Roof_Arr[i]);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("PlayerTrigger"))
        {
            for (int i = 0; i < triggerHandler.SR_WareHouse_Roof_Arr.Length; i++)
            {
                StartCoroutine("FadeIn", triggerHandler.SR_WareHouse_Roof_Arr[i]);
            }
        }
    }

    IEnumerator FadeOut(SpriteRenderer sr)
    {
        Color color = sr.color;
        start = color.a; end = 0f;
        for (float i = 0.01f; i < fadeTime; i += 0.1f)
        {
            color.a = Mathf.Lerp(start, end, i / fadeTime);
            sr.color = color;
            yield return null;
        }
    }

    IEnumerator FadeIn(SpriteRenderer sr)
    {
        Color color = sr.color;
        start = color.a; end = 1f;
        for (float i = 0.01f; i < fadeTime; i += 0.1f)
        {
            color.a = Mathf.Lerp(start, end, i / fadeTime);
            sr.color = color;
            yield return null;
        }
    }
}