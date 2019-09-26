using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarTrigger_Above : MonoBehaviour
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
            for (int i = 0; i < triggerHandler.SR_ObjAbove_List.Count; i++)
            {
                StartCoroutine("FadeOut", triggerHandler.SR_ObjAbove_List[i]);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("PlayerTrigger"))
        {
            for (int i = 0; i < triggerHandler.SR_ObjAbove_List.Count; i++)
            {
                StartCoroutine("FadeIn", triggerHandler.SR_ObjAbove_List[i]);
            }
        }
    }

    IEnumerator FadeOut(SpriteRenderer sr)
    {
        Color color = sr.color;
        start = 1; end = 0.5f;
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
        start = 0.5f; end = 1f;
        for (float i = 0.01f; i < fadeTime; i += 0.1f)
        {
            color.a = Mathf.Lerp(start, end, i / fadeTime);
            sr.color = color;
            yield return null;
        }
    }
}
