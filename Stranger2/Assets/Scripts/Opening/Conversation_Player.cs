using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Conversation_Player : MonoBehaviour
{
    GameObject TalkBox_SmugBroker;
    public Text text;
    public string[] keys = new string[] { "Player_Key0", "Player_Key1", "Player_Key2", "Player_Key3", "Player_Key4" };
    public string[] texts;
    public int keyCounter = 0;

    void Awake()
    {
        TalkBox_SmugBroker = GameObject.Find("TalkBox_SmugBroker");
        texts = new string[keys.Length];
        text = this.transform.GetChild(2).GetComponent<Text>();
        StartCoroutine("GetLocalizedText");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (keyCounter < keys.Length)
            {
                this.gameObject.SetActive(false);
                TalkBox_SmugBroker.SetActive(true);
                keyCounter++;
            }
        }
    }

    private void OnEnable()
    {
        if (keyCounter < keys.Length)
        {
            text.text = texts[keyCounter];
        }
        else return;
    }

    IEnumerator GetLocalizedText()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < keys.Length; i++)
        {
            texts[i] = LocalizationManager.instance.GetLocalizedValue(keys[i]);
        }
        this.gameObject.SetActive(false);
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

}