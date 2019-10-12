using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Conversation_SmugBroker : MonoBehaviour
{
    GameObject talkBox_Player;
    Opening_BrokerContoller brokerController;
    public Text text;
    public string[] keys = new string[] { "SmugBroker_Key0", "SmugBroker_Key1", "SmugBroker_Key2", "SmugBroker_Key3", "SmugBroker_Key4" };
    public string[] texts;
    public int keyCounter = 0;

    void Awake()
    {
        talkBox_Player = GameObject.Find("TalkBox_Player");
        brokerController = GameObject.Find("OP_SmugBroker").GetComponent<Opening_BrokerContoller>();
        texts = new string[keys.Length];
        text = this.transform.GetChild(2).GetComponent<Text>();
        StartCoroutine("GetLocalizedText");
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(keyCounter < keys.Length-1)
            {
                this.gameObject.SetActive(false);
                talkBox_Player.SetActive(true);
                keyCounter++;
            }
            else if(keyCounter == keys.Length-1)
            {
                this.gameObject.SetActive(false);
                brokerController.PlayMoveForwardAnim();
            }
        }
    }

    private void OnEnable()
    {
        if(texts[keyCounter] != null)
        {
            if (keyCounter < keys.Length)
            {
                text.text = texts[keyCounter];
            }
            else return;
        }
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
