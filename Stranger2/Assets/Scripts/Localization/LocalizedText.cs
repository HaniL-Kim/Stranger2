using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;
    private Text text;

    private void OnEnable()
    {
        text = GetComponent<Text>();
        StartCoroutine("ReloadText");
        // ReloadText();
    }

    /*
    public void ReloadText()
    {
        if (text != null)
        {
            text.text = LocalizationManager.instance.GetLocalizedValue(key);
        }
    }
    */

    IEnumerator ReloadText()
    {
        while (LocalizationManager.instance == null)
        {
            yield return null;
        }
        while (LocalizationManager.instance.localizedText == null)
        {
            yield return null;
        }
        text.text = LocalizationManager.instance.GetLocalizedValue(key);
    }
}