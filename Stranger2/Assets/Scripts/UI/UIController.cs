using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Texture2D[] cursor = new Texture2D[2];
    // MainScene
    private GameObject inGamePanel;
        private GameObject playerImage;
    private GameObject pausePanel;
        private GameObject optionPanel;
            private GameObject optionOuterButton;
            private GameObject optionOpenedButton;
            private GameObject optionButton;
        private GameObject settingPanel; // Main & Menu Scene
        public GameObject Del_Panel_Button;
    private GameObject inventoryPanel;
        private GameObject languagePanel; private GameObject languageDescriptionText;
        private GameObject selectPanel; private GameObject LanguageArrowButton_Right; private GameObject LanguageArrowButton_Left;
    // MenuScene
    private GameObject menuPanel;
    private GameObject creditPanel;
    private GameObject savePanel; private GameObject saveSlot1Panel; private GameObject saveSlot2Panel; private GameObject saveSlot3Panel;

    private void Awake()
    {
        StartCoroutine("SetCustomCursor");
        
        if (SceneManager.GetActiveScene().name == "2_MainScene")
        {
            inGamePanel = this.transform.GetChild(0).gameObject; // Depth1 A
                playerImage = inGamePanel.transform.GetChild(0).gameObject; // Depth2 Ab

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


            inventoryPanel = this.transform.GetChild(2).gameObject; // Depth2 Aa

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

            savePanel = menuPanel.transform.GetChild(1).gameObject; // Depth2 Ab
            languagePanel = settingPanel.transform.GetChild(1).gameObject; // Depth2 Bb

            saveSlot1Panel = savePanel.transform.GetChild(0).gameObject; // Depth2 Aba
            saveSlot2Panel = savePanel.transform.GetChild(1).gameObject; // Depth2 Abb
            saveSlot3Panel = savePanel.transform.GetChild(2).gameObject; // Depth2 Abc
            languageDescriptionText = languagePanel.transform.GetChild(1).gameObject; // Depth3 Bbb
            languageDescriptionText.SetActive(false);

            settingPanel.SetActive(false);
            creditPanel.SetActive(false);
            saveSlot1Panel.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false); // SaveSlot1 Continue
            saveSlot1Panel.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false); // SaveSlot1 Delete
            saveSlot2Panel.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false); // SaveSlot2 Continue
            saveSlot2Panel.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false); // SaveSlot2 Delete
            saveSlot3Panel.transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false); // SaveSlot3 Continue
            saveSlot3Panel.transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false); // SaveSlot3 Delete
            /* TODO
            * if(세이브 데이터가 있을때)
            * NewGame 버튼 비활성화
            * Continue & Delete 버튼 활성화
            */
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

    IEnumerator SetCustomCursor()
    {
        yield return new WaitForEndOfFrame();
        Cursor.SetCursor(cursor[0], Vector2.zero, CursorMode.ForceSoftware);
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
        if (SceneManager.GetActiveScene().name == "2_MainScene")
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
        if (SceneManager.GetActiveScene().name == "2_MainScene")
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
        Debug.Log("BGM Value Changed");
    }

    public void EFFSlider()
    {
        Debug.Log("EFF Value Changed");
    }

    public void InventoryItemButton()
    {
        Debug.Log("Inventory Item Clicked");
    }

    public void GoMainButton()
    {
        Debug.Log("Load MenuScene");
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

    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 유니티 에디터에서
#else
        Application.Quit(); // PC에서
#endif
    }
}