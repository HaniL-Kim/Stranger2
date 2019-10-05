using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Guard_TalkBalloon : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    string[] guard_Type1_keys = new string[6]
    { "guard_Type1_key0", "guard_Type1_key1","guard_Type1_key2","guard_Type1_key3","guard_Type1_key4","guard_Type1_key5" };
    string[] guard_Type2_keys = new string[3]
     { "guard_Type2_key0","guard_Type2_key1","guard_Type2_key2" };
    public string[] keys = null;
    public float delay;

    void Awake()
    {
        textMeshPro = this.GetComponent<TextMeshProUGUI>();
        if (this.transform.parent.parent.parent.parent.name.EndsWith("Type1"))
        {
            keys = guard_Type1_keys;
        }
        else if (this.transform.parent.parent.parent.parent.name.EndsWith("Type2"))
        {
            keys = guard_Type2_keys;
        }

        this.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInParent<RectTransform>());
        StartCoroutine("GuardTalk");

    }

    IEnumerator GuardTalk()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < keys.Length; i++)
        {
            textMeshPro.text = LocalizationManager.instance.GetLocalizedValue(keys[i]);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInParent<RectTransform>());
            yield return new WaitForSeconds(delay);
        }
        StartCoroutine("GuardTalk");
    }
}