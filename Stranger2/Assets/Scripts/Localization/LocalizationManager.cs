using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;
    public Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized file not found";
    public string localizedFileName;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            StartCoroutine("Initialize");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    IEnumerator Initialize()
    {
        Debug.Log("Initialize()");
        while (!SaveDataController.instance.isReady)
        {
            Debug.Log("SaveDataController is Not Ready");
            yield return null;
        }
        while(SaveDataController.instance.languageSelected == null)
        {
            Debug.Log("languageData is null");
            yield return null;
        }
        localizedFileName = SaveDataController.instance.languageSelected;
        LoadLocalizedText();
    }

    public void LanguageArrowButton()
    { // in Setting / Reload Text
        if (localizedFileName == "localizedText_Kor.json")
        {
            localizedFileName = "localizedText_Eng.json";
            PlayerPrefs.SetString("languageSelected", localizedFileName);
            LoadLocalizedText();
        }
        else
        {
            localizedFileName = "localizedText_Kor.json";
            PlayerPrefs.SetString("languageSelected", localizedFileName);
            LoadLocalizedText();
        }
        foreach (LocalizedText text in FindObjectsOfType<LocalizedText>())
        {
            text.StartCoroutine("ReloadText");
            // ReloadText();
        }
    }

    public void SelectLanguageButton(string fileNameToLocalize)
    { // in Fisrt Scene
        localizedFileName = fileNameToLocalize;
        SaveDataController.instance.languageSelected = localizedFileName;
        PlayerPrefs.SetString("languageSelected", localizedFileName);
        LoadLocalizedText();
    }

    public void LoadLocalizedText()
    {
        localizedText = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, localizedFileName);
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
            for (int i = 0; i < loadedData.items.Length; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
            Debug.Log("Data loaded, dictionary contains : " + localizedText.Count + " entries");
        }
        else
        {
            Debug.LogError("Can't Find File");
        }
        isReady = true;
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;

        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
            result.Replace("\\n", "\n");
        }
        return result;
    }

    public bool GetIsReady()
    {
        return isReady;
    }

} // End Of Script