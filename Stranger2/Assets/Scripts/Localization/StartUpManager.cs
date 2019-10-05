using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUpManager : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        while (!LocalizationManager.instance.GetIsReady())
        {
            yield return null;
        }

        SceneManager.LoadScene("1_MenuScene");
    }
}