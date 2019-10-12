using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningController : MonoBehaviour
{
    UIController uIController;
    Transform TF_cam;
    GameObject cut_1;
    GameObject cut_2;
    GameObject cut_3;
    public SpriteRenderer sr_cut2_BlackBG;
    public SpriteRenderer sr_cut3_BlackBG;
    public SpriteRenderer[] sr_cut2_Obj;
    public SpriteRenderer[] sr_cut3_Obj;

    public float fadeTime = 2f;

    Color alphaZero;

    void Awake()
    {
        uIController = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UIController>();
        TF_cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        cut_1 = this.transform.GetChild(0).gameObject;
        cut_2 = this.transform.GetChild(1).gameObject;
        cut_3 = this.transform.GetChild(2).gameObject;
        alphaZero = new Color(1f, 1f, 1f, 0f);
        InitializeCut2();
        InitializeCut3();
    }

    void InitializeCut2()
    {
        sr_cut2_BlackBG = cut_2.transform.GetChild(0).GetComponent<SpriteRenderer>();
        sr_cut2_Obj = cut_2.transform.GetChild(1).GetComponentsInChildren<SpriteRenderer>();

        sr_cut2_BlackBG.color = alphaZero;
        for (int i = 0; i < sr_cut2_Obj.Length; i++)
        {
            sr_cut2_Obj[i].color = new Color(sr_cut2_Obj[i].color.r, sr_cut2_Obj[i].color.g, sr_cut2_Obj[i].color.b, 0f);
        }
    }

    void InitializeCut3()
    {
        sr_cut3_BlackBG = cut_3.transform.GetChild(0).GetComponent<SpriteRenderer>();
        sr_cut3_Obj = cut_3.transform.GetChild(1).GetComponentsInChildren<SpriteRenderer>();

        sr_cut3_BlackBG.color = alphaZero;
        for (int i = 0; i < sr_cut3_Obj.Length; i++)
        {
            sr_cut3_Obj[i].color = new Color(sr_cut3_Obj[i].color.r, sr_cut3_Obj[i].color.g, sr_cut3_Obj[i].color.b, 0f);
        }
    }

    IEnumerator StartCut2()
    {
        StartCoroutine("FadeIn", sr_cut2_BlackBG);
        yield return new WaitForSeconds(2.5f);
        TF_cam.position = new Vector3(0, 0, -1f);
        cut_1.SetActive(false);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < sr_cut2_Obj.Length; i++)
        {
            StartCoroutine("FadeIn", sr_cut2_Obj[i]);
        }
        yield return new WaitForSeconds(3f);
        Vector3 targetPos = new Vector3(0, 0f, -10f);
        int timeOfLerp = 60;
        for (int i = 0; i < timeOfLerp; i++)
        {
            float t = (float)i / (float)(timeOfLerp - 1);
            
            TF_cam.position = new Vector3(0, 0, targetPos.z * t);
            // TF_cam.position = Vector3.Slerp(TF_cam.transform.position, endPos, t);
            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine("StartCut3");
    }

    IEnumerator StartCut3()
    {
        StartCoroutine("FadeIn", sr_cut3_BlackBG);
        yield return new WaitForSeconds(2.5f);
        TF_cam.position = new Vector3(0, 0, -1f);
        cut_2.SetActive(false);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < sr_cut3_Obj.Length; i++)
        {
            StartCoroutine("FadeIn", sr_cut3_Obj[i]);
        }
        yield return new WaitForSeconds(6f);
        StartPage();
    }

    void StartPage()
    {
        uIController.blackBGPanel.SetActive(true);
        Image blackBGPanelImg = uIController.blackBGPanel.GetComponent<Image>();
        Transform stageTexts = uIController.blackBGPanel.transform.GetChild(0);
        Transform Stagethumbnail = uIController.blackBGPanel.transform.GetChild(1);

        blackBGPanelImg.canvasRenderer.SetAlpha(0f); // CrossFadeAlpha doesn't change the alpha value of the image.color. fact is that it changes the CanvaseRenderer.
        blackBGPanelImg.CrossFadeAlpha(1f, 2f, false);

        stageTexts.GetChild(0).GetComponent<Text>().text = "Chapter0"; // Chapter0
        stageTexts.GetChild(1).GetComponent<LocalizedText>().key = "StageNameText_0"; // StageNameText_0
        stageTexts.GetChild(1).GetComponent<LocalizedText>().StartCoroutine("ReloadText");
        stageTexts.GetChild(2).GetComponent<LocalizedText>().key = "StageDescriptionText_0"; // StageDescriptionText_0
        stageTexts.GetChild(2).GetComponent<LocalizedText>().StartCoroutine("ReloadText");
        Stagethumbnail.GetComponent<Image>().sprite = uIController.sp_Thumbnails[0];

        for (int i = 0; i < stageTexts.childCount; i++)
        {
            stageTexts.GetChild(i).GetComponent<CanvasRenderer>().SetAlpha(0f);
            stageTexts.GetChild(i).GetComponent<Text>().CrossFadeAlpha(1f, 2f, false);
        }
        Stagethumbnail.GetComponent<CanvasRenderer>().SetAlpha(0f);
        Stagethumbnail.GetComponent<Image>().CrossFadeAlpha(1f, 2f, false);
        uIController.StartCoroutine("PressAnyKey");
    }

    IEnumerator FadeIn(SpriteRenderer sr)
    {
        Color color = sr.color;
        float time = 0; float start = 0; float end = 1;
        while (color.a < 1f)
        {
            time += Time.deltaTime / fadeTime;
            color.a = Mathf.Lerp(start, end, time);
            sr.color = color;
            yield return null;
        }
    }

}
