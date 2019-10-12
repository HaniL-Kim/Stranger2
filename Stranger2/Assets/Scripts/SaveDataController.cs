using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveDataController : MonoBehaviour
{
    public static SaveDataController instance;
    public UIController uIController;
    public GameObject UICanvas;
    public Slider BGM_Slider;
    public Slider EFF_Slider;
    public string languageSelected = null;
    public float BGM_value;
    public float EFF_value;
    bool hasSettingData;

    public bool[] usingSaveSlot = new bool[3];
    public int[] stages = new int[3];

    public string saveSlotNow;

    public bool isReady = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
            LoadSettingData();
            LoadSaveData();
        }
        else if (instance != this)
        {
            instance.Initialize();
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Initialize()
    {
        uIController = FindObjectOfType<UIController>();

        if (SceneManager.GetActiveScene().name == "1_MenuScene")
        {
            UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
            BGM_Slider = UICanvas.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Slider>();
            EFF_Slider = UICanvas.transform.GetChild(2).GetChild(3).GetChild(1).GetComponent<Slider>();
        }
        if (SceneManager.GetActiveScene().name == "2_MainScene_Stage0")
        {
            UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
            BGM_Slider = UICanvas.transform.GetChild(1).GetChild(1).GetChild(2).GetChild(1).GetComponent<Slider>();
            EFF_Slider = UICanvas.transform.GetChild(1).GetChild(1).GetChild(3).GetChild(1).GetComponent<Slider>();
        }
    }

    public void SaveSettingData()
    {
        PlayerPrefs.SetString("languageSelected", LocalizationManager.instance.localizedFileName); // LocalizedFileName
        PlayerPrefs.SetFloat("BGM_value", BGM_Slider.value); // BGM
        PlayerPrefs.SetFloat("EFF_value", EFF_Slider.value); // BGM
        PlayerPrefs.Save();
    }

    public void LoadSettingData()
    {
        if (!PlayerPrefs.HasKey("languageSelected"))
        {
            hasSettingData = false;
        }
        else
        {
            hasSettingData = true;
        }

        if (hasSettingData)
        {
            languageSelected = PlayerPrefs.GetString("languageSelected");
            BGM_value = PlayerPrefs.GetFloat("BGM_value");
            EFF_value = PlayerPrefs.GetFloat("EFF_value");
        }
        else
        { // SetDefault
            languageSelected = null;
            BGM_value = 10f;
            EFF_value = 10f;
        }
        isReady = true;
    }

    public void LoadSaveData()
    {
        for (int i = 0; i < 3; i++)
        {
            if (PlayerPrefs.HasKey(i.ToString() + "SaveSlot"))
            {
                if (PlayerPrefs.GetInt(i.ToString() + "SaveSlot") == 1)
                {
                    usingSaveSlot[i] = true;
                    stages[i] = PlayerPrefs.GetInt(i.ToString() + "SaveSlotStage");
                }
                else
                {
                    usingSaveSlot[i] = false;
                }

            }

        }
    }

    public void DeleteSaveData(int numToDeleteSlot)
    {
        PlayerPrefs.DeleteKey(numToDeleteSlot.ToString() + "SaveSlot");
        PlayerPrefs.DeleteKey(numToDeleteSlot.ToString() + "SaveSlotStage");
    }

}
