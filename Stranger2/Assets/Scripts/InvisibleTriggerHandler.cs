using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleTriggerHandler : MonoBehaviour
{
    private GameObject WareHouse;
    public GameObject WareHouseRoof;
    public GameObject WareHouse_Pillar;
    public SpriteRenderer[] SR_WareHouseRoof_Arr;
    public SpriteRenderer[] SR_WareHouse_Pillar_Arr;

    // public bool isFading;
    public float fadeTime = 1f;
    float start;
    float end;
    float time = 0f;

    private void Awake()
    {
        WareHouse = this.transform.parent.gameObject;
        WareHouseRoof = WareHouse.transform.GetChild(1).gameObject;
        WareHouse_Pillar = WareHouse.transform.GetChild(2).gameObject;
        SR_WareHouseRoof_Arr = WareHouseRoof.GetComponentsInChildren<SpriteRenderer>();
        SR_WareHouse_Pillar_Arr = WareHouse_Pillar.GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerTrigger"))
        {
            /*
            for (int i = 0; i < SR_WareHouse_Pillar_Arr.Length; i++)
            {
                SR_WareHouse_Pillar_Arr[i].color = alphaReduced;
            }
            */
            for (int i = 0; i < SR_WareHouseRoof_Arr.Length; i++)
            {
                StartCoroutine("FadeIn", SR_WareHouseRoof_Arr[i]);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("PlayerTrigger"))
        {
            /*
            for (int i = 0; i < SR_WareHouse_Pillar_Arr.Length; i++)
            {
                SR_WareHouse_Pillar_Arr[i].color = Color.white;
            }
            */
            for (int i = 0; i < SR_WareHouseRoof_Arr.Length; i++)
            {
                StartCoroutine("FadeOut", SR_WareHouseRoof_Arr[i]);
            }
        }
    }

    IEnumerator FadeOut(SpriteRenderer sr)
    {
        Color color = sr.color;
        time = 0 ; start = color.a; end = 1;
        // color.a = Mathf.Lerp(start, end, time);
        while (color.a < 1f)
        {
            time += Time.deltaTime / fadeTime;
            color.a = Mathf.Lerp(start, end, time);
            sr.color = color;
            yield return null;
        }
    }
    IEnumerator FadeIn(SpriteRenderer sr)
    {
        Color color = sr.color;
        time = 0; start = color.a; end = 0;
        /*
        time = 0f; start = 1; end = 0;
        color.a = Mathf.Lerp(start, end, time);
        */
        while (color.a > 0f)
        {
            time += Time.deltaTime / fadeTime;
            color.a = Mathf.Lerp(start, end, time);
            sr.color = color;
            yield return null;
        }
    }
}
