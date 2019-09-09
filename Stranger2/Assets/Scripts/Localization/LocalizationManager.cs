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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        localizedFileName = "localizedText_Kor.json";
    }

    public void LanguageArrowButton()
    { // in Setting / Reload Text
        if (localizedFileName == "localizedText_Kor.json")
        {
            localizedFileName = "localizedText_Eng.json";
            LoadLocalizedText();
        }
        else
        {
            localizedFileName = "localizedText_Kor.json";
            LoadLocalizedText();
        }
        foreach (LocalizedText text in FindObjectsOfType<LocalizedText>())
        {
            text.ReloadText();
        }
    }

    public void SelectLanguageButton(string fileNameToLocalize)
    { // in Fisrt Scene
        localizedFileName = fileNameToLocalize;
        LoadLocalizedText();
    }

    public void LoadLocalizedText()
    {
        Debug.Log(localizedFileName);
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

        if(localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }

        return result;
    }

    public bool GetIsReady()
    {
        return isReady;
    }

}
