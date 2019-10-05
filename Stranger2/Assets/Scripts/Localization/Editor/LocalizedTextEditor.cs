using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizedTextEditor : EditorWindow
{
    public LocalizationData localizationData;

    [MenuItem("Window/Localized Text Editor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(LocalizedTextEditor)).Show();
    }

    private void OnGUI()
    {
        if(localizationData != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("localizationData");
            EditorGUILayout.PropertyField(serializedProperty, true);
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Save data"))
            {
                SaveGameData();
            }
        }
        if (GUILayout.Button("Load data"))
        {
            LoadGameData();
        }

        if (GUILayout.Button("Create new data"))
        {
            CreateNewData();
        }
    }

    private void LoadGameData()
    {
        string filePath = EditorUtility.OpenFilePanel("select localization data file", Application.streamingAssetsPath, "json");
        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            localizationData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
        }
    }

    private void SaveGameData()
    {
        string filePath = EditorUtility.SaveFilePanel("save localization data file", Application.streamingAssetsPath, "", "json");
        if(!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData);
            string[] spe = { ",{" };
            string[] datas = dataAsJson.Split(spe, System.StringSplitOptions.None);

            for (int i = 0; i < datas.Length; i++)
            {
                if( i == datas.Length -1)
                {
                    break;
                }
                else
                {
                    datas[i] = datas[i] + "\n,{";
                }
            }
            dataAsJson = null;
            for (int i = 0; i < datas.Length; i++)
            {
                dataAsJson += datas[i];
            }

            File.WriteAllText(filePath, dataAsJson);
        }
    }

    private void CreateNewData()
    {
        localizationData = new LocalizationData();
    }
}