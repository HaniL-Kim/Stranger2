using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AlphaText : MonoBehaviour
{
    public float speedFade;
    Text text;
    bool isBlink;

    void Awake()
    {
        text = GetComponent<Text>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }

    void Update()
    {
        if(isBlink)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene("2_MainScene_Stage0");
            }
        }
    }

    IEnumerator Blink()
    {
        while(true)
        {
            switch (text.canvasRenderer.GetAlpha())
            {
                case 0:
                    text.canvasRenderer.SetAlpha(0f); // CrossFadeAlpha doesn't change the alpha value of the image.color. fact is that it changes the CanvaseRenderer.
                    text.CrossFadeAlpha(1f, speedFade, true);
                    yield return new WaitForSeconds(speedFade);
                    break;
                case 1:
                    text.canvasRenderer.SetAlpha(1f); // CrossFadeAlpha doesn't change the alpha value of the image.color. fact is that it changes the CanvaseRenderer.
                    text.CrossFadeAlpha(0f, speedFade, true);
                    yield return new WaitForSeconds(speedFade);
                    break;
                default:
                    break;
            }
        }
    }

    public void StartBlink()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        isBlink = true;
        StartCoroutine("Blink");
    }
    public void StopBlink()
    {
        isBlink = false;
        StopCoroutine("Blink");
    }

}
