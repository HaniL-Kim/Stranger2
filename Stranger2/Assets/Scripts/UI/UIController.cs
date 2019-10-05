using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    GameObject player;
    public Texture2D[] cursor = new Texture2D[2];
    // MainScene
    private GameObject inGamePanel;
    private GameObject playerImage;    public GameObject playerTextBalloon;
    private GameObject pausePanel;
    private GameObject optionPanel;    private GameObject optionOuterButton;    private GameObject optionOpenedButton;    private GameObject optionButton;
    private GameObject settingPanel; // Main & Menu Scene
    public GameObject Del_Panel_Button;
    private GameObject inventoryPanel;
    private GameObject languagePanel; private GameObject languageDescriptionText;
    private GameObject selectPanel; private GameObject LanguageArrowButton_Right; private GameObject LanguageArrowButton_Left;
    // MenuScene
    private GameObject menuPanel;
    private GameObject creditPanel;
    public GameObject savePanel;
    private GameObject deleteEnsurePanel;
    private GameObject gameOverPanel;

    private GameObject blackBGPanel;
    public Sprite[] sp_Thumbnails;
    public Sprite pixelGray;
    public GameObject deleteButtonSelected;

    private void Awake()
    {
        StartCoroutine("SetCustomCursor");
        if (SceneManager.GetActiveScene().name == "2_MainScene_Stage0")
        {
            player = GameObject.FindGameObjectWithTag("Player").gameObject;
            inGamePanel = this.transform.GetChild(0).gameObject; // Depth1 A
            playerImage = inGamePanel.transform.GetChild(0).gameObject; // Depth2 Ab
            playerTextBalloon = inGamePanel.transform.GetChild(1).gameObject; // Depth2 Ab

            pausePanel = this.transform.GetChild(1).gameObject; // Depth1 B
            optionPanel = pausePanel.transform.GetChild(0).gameObject; // Depth2 Ba
            optionOuterButton = optionPanel.transform.GetChild(0).gameObject; // Depth3 BbA
            optionOpenedButton = optionPanel.transform.GetChild(1).gameObject; // Depth3 BbB
            optionButton = optionPanel.transform.GetChild(2).gameObject; // Depth3 BbC
            settingPanel = pausePanel.transform.GetChild(1).gameObject; // Depth2 Bb
            languagePanel = settingPanel.transform.GetChild(1).gameObject; // Depth3 BcB
            languageDescriptionText = languagePanel.transform.GetChild(1).gameObject; // Depth4 BcBb
            selectPanel = languagePanel.transform.GetChild(2).gameObject; // Depth4 BcBc
            LanguageArrowButton_Right = selectPanel.transform.GetChild(0).gameObject; // Depth5 BcBcA
            LanguageArrowButton_Left = selectPanel.transform.GetChild(1).gameObject; // Depth5 BcBcB
            Del_Panel_Button = pausePanel.transform.GetChild(2).gameObject; // Depth2 Bc

            inventoryPanel = this.transform.GetChild(2).gameObject; // Depth1 C
            gameOverPanel = this.transform.GetChild(3).gameObject; // Depth1 D

            playerTextBalloon.SetActive(false);
            gameOverPanel.SetActive(false);
            pausePanel.SetActive(false);
            inventoryPanel.GetComponent<Inventory>().SlotInteractToggle();
            settingPanel.SetActive(false);
            optionOuterButton.SetActive(false);
            optionOpenedButton.SetActive(false);

            LanguageArrowButton_Right.SetActive(false);
            LanguageArrowButton_Left.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "1_MenuScene")
        {
            menuPanel = this.transform.GetChild(0).gameObject; // Depth1 A
            settingPanel = this.transform.GetChild(1).gameObject; // Depth1 B
            creditPanel = this.transform.GetChild(2).gameObject; // Depth1 C
            deleteEnsurePanel = this.transform.GetChild(3).gameObject; // Depth1 D
            blackBGPanel = this.transform.GetChild(4).gameObject; // Depth1 E

            savePanel = menuPanel.transform.GetChild(1).gameObject; // Depth2 Ab
            languagePanel = settingPanel.transform.GetChild(1).gameObject; // Depth2 Bb

            languageDescriptionText = languagePanel.transform.GetChild(1).gameObject; // Depth3 Bbb
            languageDescriptionText.SetActive(false);

            settingPanel.SetActive(false);
            creditPanel.SetActive(false);
            deleteEnsurePanel.SetActive(false);
            blackBGPanel.SetActive(false);

            StartCoroutine("SetVolume");
            StartCoroutine("SetSaveSlot");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursor[1], Vector2.zero, CursorMode.ForceSoftware);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(cursor[0], Vector2.zero, CursorMode.ForceSoftware);
        }
    }

    public IEnumerator StartReadBoard(TriggerHandler_Board board)
    {
        playerTextBalloon.SetActive(true);

        RectTransform balloonRect = playerTextBalloon.transform.GetChild(0).GetComponent<RectTransform>();
        float width = balloonRect.rect.width;
        float height = balloonRect.rect.height;
        Vector2 initialSize = new Vector2(0, height);
        balloonRect.sizeDelta = initialSize;

        for (int i = 0; i < 50; i++)
        {
            initialSize.x += 5f;
            balloonRect.sizeDelta = initialSize;
            yield return new WaitForSeconds(0.01f);
        }

        board.isReading = true;
        playerTextBalloon.GetComponentInChildren<Text>().text = board.textInBoard[0];
        yield return null;
    }
    public IEnumerator ContinueReadBoard(TriggerHandler_Board board)
    {
        playerTextBalloon.GetComponentInChildren<Text>().text = board.textInBoard[board.keyCounter];
        yield return null;
    }
    public IEnumerator EndReadBoard(TriggerHandler_Board board)
    {
        playerTextBalloon.GetComponentInChildren<Text>().text = null;
        RectTransform balloonRect = playerTextBalloon.transform.GetChild(0).GetComponent<RectTransform>();
        float width = balloonRect.rect.width;
        float height = balloonRect.rect.height;
        Vector2 initialSize = new Vector2(width, height);
        for (int i = 0; i < 50; i++)
        {
            initialSize.x -= 5f;
            balloonRect.sizeDelta = initialSize;
            yield return new WaitForSeconds(0.01f);
        }
        board.isReading = false;
        playerTextBalloon.SetActive(false);
        yield return null;
    }

    public void ResetSaveSlot()
    {
        string selectedSlotString = deleteButtonSelected.name;
        int selectedSlotNum = int.Parse(selectedSlotString.Substring(selectedSlotString.Length - 1));
        SaveDataController.instance.usingSaveSlot[selectedSlotNum] = false;
        bool usingSlot = false;
        Transform TF_SaveSlot = savePanel.transform.GetChild(selectedSlotNum);
        TF_SaveSlot.GetChild(0).GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 100 / 255f);
        TF_SaveSlot.GetChild(0).GetChild(1).GetComponent<Image>().sprite = pixelGray;
        TF_SaveSlot.GetChild(0).GetChild(2).GetComponent<Text>().text = "Empty Slot";
        for (int j = 0; j < 3; j++)
        {
            TF_SaveSlot.GetChild(1).GetChild(j).gameObject.SetActive(!usingSlot); // SaveSlot1 New
            if (j == 0)
            {
                usingSlot = !usingSlot;
            }
        }

        SaveDataController.instance.DeleteSaveData(selectedSlotNum);

    }

    IEnumerator SetSaveSlot()
    {
        while (SaveDataController.instance == null)
        { // for Debug(only at Excute Scene_1 in Editor
            yield return null;
        }
        while (SaveDataController.instance.usingSaveSlot == null)
        {
            yield return null;
        }
        for (int i = 0; i < 3; i++)
        {
            bool usingSlot = SaveDataController.instance.usingSaveSlot[i];
            Transform TF_SaveSlot = savePanel.transform.GetChild(i);
            if (usingSlot)
            {
                TF_SaveSlot.GetChild(0).GetChild(1).GetComponent<Image>().color = Color.white;
                TF_SaveSlot.GetChild(0).GetChild(1).GetComponent<Image>().sprite = sp_Thumbnails[SaveDataController.instance.stages[i]];
                TF_SaveSlot.GetChild(0).GetChild(2).GetComponent<Text>().text = "Chapter" + SaveDataController.instance.stages[i].ToString();
            }
            for (int j = 0; j < 3; j++)
            {
                TF_SaveSlot.GetChild(1).GetChild(j).gameObject.SetActive(!usingSlot); // SaveSlot1 New

                if (j == 0)
                {
                    usingSlot = !usingSlot;
                }
            }
        }
    }


    IEnumerator SetVolume()
    {
        while (SaveDataController.instance == null)
        { // for Debug(only at Excute Scene_1 in Editor
            yield return null;
        }
        while (SaveDataController.instance.BGM_Slider == null || SaveDataController.instance.EFF_Slider == null)
        {
            yield return null;
        }
        if (PlayerPrefs.HasKey("BGM_value"))
        {
            SaveDataController.instance.BGM_Slider.value = PlayerPrefs.GetFloat("BGM_value");
        }
        if (PlayerPrefs.HasKey("EFF_value"))
        {
            SaveDataController.instance.EFF_Slider.value = PlayerPrefs.GetFloat("EFF_value");
        }
    }

    IEnumerator SetCustomCursor()
    {
        yield return new WaitForEndOfFrame();
        if (cursor[0] == null)
        {
            cursor[0] = Resources.Load<Texture2D>("Sprite/Cursor_Normal");
            cursor[1] = Resources.Load<Texture2D>("Sprite/Cursor_ButtonDown");
        }
        Cursor.SetCursor(cursor[0], Vector2.zero, CursorMode.ForceSoftware);
    }

    IEnumerator PressAnyKey()
    {
        yield return new WaitForSeconds(4f);
        FindObjectOfType<AlphaText>().StartBlink();
    }

    public void StartContinue()
    {
        blackBGPanel.SetActive(true);
        Image blackBGPanelImg = blackBGPanel.GetComponent<Image>();
        Transform stageTexts = blackBGPanel.transform.GetChild(0);
        Transform Stagethumbnail = blackBGPanel.transform.GetChild(1);

        blackBGPanelImg.canvasRenderer.SetAlpha(0f); // CrossFadeAlpha doesn't change the alpha value of the image.color. fact is that it changes the CanvaseRenderer.
        blackBGPanelImg.CrossFadeAlpha(1f, 2f, false);

        string continueWith = EventSystem.current.currentSelectedGameObject.name; // if) start with "Continue_0"
        int continueNum = int.Parse(continueWith.Substring(continueWith.Length - 1)); // -> 0
        int stageLevel = SaveDataController.instance.stages[continueNum]; // 0

        stageTexts.GetChild(0).GetComponent<Text>().text = "Chapter" + stageLevel.ToString(); // Chapter0
        stageTexts.GetChild(1).GetComponent<LocalizedText>().key = "StageNameText_" + stageLevel.ToString(); // StageNameText_0
        stageTexts.GetChild(1).GetComponent<LocalizedText>().StartCoroutine("ReloadText");
        stageTexts.GetChild(2).GetComponent<LocalizedText>().key = "StageDescriptionText_" + stageLevel.ToString(); // StageDescriptionText_0
        stageTexts.GetChild(2).GetComponent<LocalizedText>().StartCoroutine("ReloadText");
        Stagethumbnail.GetComponent<Image>().sprite = sp_Thumbnails[stageLevel];

        for (int i = 0; i < stageTexts.childCount; i++)
        {
            stageTexts.GetChild(i).GetComponent<CanvasRenderer>().SetAlpha(0f);
            stageTexts.GetChild(i).GetComponent<Text>().CrossFadeAlpha(1f, 2f, false);
        }
        Stagethumbnail.GetComponent<CanvasRenderer>().SetAlpha(0f);
        Stagethumbnail.GetComponent<Image>().CrossFadeAlpha(1f, 2f, false);
        StartCoroutine("PressAnyKey");
    }

    public void StartNewGame()
    {
        SaveDataController.instance.saveSlotNow = EventSystem.current.currentSelectedGameObject.name;
        Image[] imgToFade = menuPanel.GetComponentsInChildren<Image>();
        for (int i = 0; i < imgToFade.Length; i++)
        {
            imgToFade[i].CrossFadeAlpha(0, 2, false);
            if (imgToFade[i].transform.GetComponent<Button>() != null)
            {
                imgToFade[i].transform.GetComponent<Button>().enabled = false;
            }
        }
        StartCoroutine("StartMainWithDelay");
    }
    public IEnumerator StartMainWithDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("2_MainScene_Stage0");
    }

    public void EnsureDelete_Yes()
    {
        Debug.Log("EnsureDelete_Yes()");
        ResetSaveSlot();
        deleteEnsurePanel.SetActive(false);
    }

    public void EnsureDelete_No()
    {
        Debug.Log("EnsureDelete_No()");
        deleteEnsurePanel.SetActive(false);
    }

    public void EnsureDelete()
    {
        deleteButtonSelected = EventSystem.current.currentSelectedGameObject;
        deleteEnsurePanel.SetActive(true);
    }

    public void GameOver()
    {
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<PlayerRenderer>().enabled = false;
        StartCoroutine("YouDied");
    }

    IEnumerator YouDied()
    {
        gameOverPanel.SetActive(true);
        CanvasRenderer[] crs = gameOverPanel.transform.GetChild(1).GetComponentsInChildren<CanvasRenderer>();
        for (int i = 0; i < crs.Length; i++)
        {
            crs[i].SetAlpha(0f);
        }
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < crs.Length; i++)
        {
            if(crs[i].GetComponent<Image>() != null)
            {
                crs[i].GetComponent<Image>().CrossFadeAlpha(1f, 2f, true);
            }
            else if(crs[i].GetComponent<Text>() != null)
            {
                crs[i].GetComponent<Text>().CrossFadeAlpha(1f, 2f, true);
            }
        }
    }

    public void OptionButton()
    {
        optionOuterButton.SetActive(true);
        optionOpenedButton.SetActive(true);
        optionButton.SetActive(false);
    }

    public void OptionOpenedButton()
    {
        optionButton.SetActive(true);
        optionOuterButton.SetActive(false);
        optionOpenedButton.SetActive(false);
    }

    public void SettingButton()
    {
        if (SceneManager.GetActiveScene().name == "2_MainScene_Stage0")
        {
            settingPanel.SetActive(true);
            OptionOpenedButton();
        }
        else if (SceneManager.GetActiveScene().name == "1_MenuScene")
        {
            settingPanel.SetActive(true);
            menuPanel.SetActive(false);
        }
    }

    public void ExitSettingButton()
    {
        if (SceneManager.GetActiveScene().name == "2_MainScene_Stage0")
        {
            settingPanel.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "1_MenuScene")
        {
            menuPanel.SetActive(true);
            settingPanel.SetActive(false);
        }
    }

    public void LanguageArrowButton()
    {
        LocalizationManager.instance.LanguageArrowButton();
    }

    public void BGMSlider()
    {
        PlayerPrefs.SetFloat("BGM_value", SaveDataController.instance.BGM_Slider.value); // Save BGM Value
    }

    public void EFFSlider()
    {
        PlayerPrefs.SetFloat("EFF_value", SaveDataController.instance.EFF_Slider.value); // Save EFF Value
    }

    public void InventoryItemButton()
    {
        Debug.Log("Inventory Item Clicked");
    }

    public void SaveProgress()
    { // Todo : Save Slot Num & Progress 연동
        string slotString = SaveDataController.instance.saveSlotNow;
        string stageString = FindObjectOfType<GameController>().stage;
        int slotNum = int.Parse(slotString.Substring(slotString.Length - 1));
        int stageNum = int.Parse(stageString.Substring(stageString.Length - 1));
        SaveDataController.instance.usingSaveSlot[slotNum] = true;
        PlayerPrefs.SetInt(slotNum.ToString() + "SaveSlot", 1);
        PlayerPrefs.SetInt(slotNum.ToString() + "SaveSlotStage", stageNum);
    }

    public void GoMainButton()
    {
        Debug.Log("GoMainButton");
        SaveProgress();
        Time.timeScale = 1;
        SceneManager.LoadScene("1_MenuScene");
    }
    public void CreditButton()
    {
        creditPanel.SetActive(true);
        menuPanel.SetActive(false);
        this.GetComponent<Animator>().SetTrigger("CreditRoll");
    }
    public void ExitCredit()
    {
        menuPanel.SetActive(true);
        creditPanel.SetActive(false);
        this.GetComponent<Animator>().SetTrigger("CreditDefault");
    }

    public void Restart()
    {
        Debug.Log("Restart");
        Time.timeScale = 1;
        string sceneToRestart = FindObjectOfType<GameController>().stage;
        SceneManager.LoadScene(sceneToRestart);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 유니티 에디터에서
#else
        Application.Quit(); // PC에서
#endif
    }
}