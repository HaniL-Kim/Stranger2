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
        ReloadText();
    }

    public void ReloadText()
    {
        if (text != null)
        {
            text.text = LocalizationManager.instance.GetLocalizedValue(key);
        }
    }

}
