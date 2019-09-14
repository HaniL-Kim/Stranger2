using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Inventory_Slot : MonoBehaviour
{
    private GameObject inventoryObj;
    private Inventory _inventory;
    private GameObject UICanvas;
    private UIController uIController;

    [SerializeField] private Item item;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite emptyIcon = null;

    public KeyCode _keyCode;
    private Button _button;
    private Sprite normalStateSprite;
    [SerializeField] private GameObject _del;

    private void Awake()
    {
        UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
            uIController = UICanvas.GetComponent<UIController>();
        _del = UICanvas.transform.GetChild(1).transform.GetChild(2).gameObject;

        inventoryObj = this.transform.parent.gameObject;
        _inventory = inventoryObj.GetComponent<Inventory>();
        icon = this.transform.GetChild(0).gameObject.GetComponent<Image>();
        normalStateSprite = GetComponent<Image>().sprite;
        _button = GetComponent<Button>();

        if(item != null)
        {
            icon.sprite = item.Icon;
        }
        else
        {
            icon.sprite = emptyIcon;
        }
    }

    private void Update()
    {
        if(!this.transform.parent.GetComponent<Inventory>().isInteractable)
        {
            if (Input.GetKeyDown(_keyCode))
            {
                GetComponent<Image>().sprite = _button.spriteState.pressedSprite;
            }
            else if (Input.GetKeyUp(_keyCode))
            {
                GetComponent<Image>().sprite = normalStateSprite;
                UseItem();
            }
        }
    }

    private void UseItem()
    {
        if(item != null)
        {
            Debug.Log(item.name + "이 사용됨");
        }
        else
            Debug.Log("가진 것이 아무것도 없습니다.");
        
    }

    public void ChangeSlot()
    {
        if (!_inventory.isSelect)
        { // 선택중이 아닐 때
            if (item != null)
            { // slot에 Item이 있을 때
                _inventory.itemContainerOrigin = item; // 인벤토리에 아이템 보관
                _inventory.selectedIcon.sprite = icon.sprite; // 커서 이미지 설정
                _inventory.isSelect = true; // 이미지 위치 update 활성화
                _del.GetComponent<Button>().interactable = true; // Del Button 활성화
                item = null; // 슬롯 비우기
                icon.sprite = emptyIcon; // 슬롯 아이콘 변경(Empty)
                Cursor.visible = false; // 커서 비 가시화
            }
            else
            { // slot에 Item이 없을 때
                return;
            }
        }
        else
        { // 선택중일 때
            if (item != null)
            { // slot에 Item이 있을 때
                _inventory.selectedIcon.sprite = item.Icon; // 커서 이미지 설정

                _inventory.itemContainerDestin = item; // 인벤토리 두번째 칸에 아이템 보관
                item = _inventory.itemContainerOrigin; // 인벤토리 첫번째 칸에 갖고있던 아이템 슬롯에 배치
                icon.sprite = item.Icon; // 아이콘 변경

                _inventory.itemContainerOrigin = _inventory.itemContainerDestin; // 아이템 보관위치 변경
                _inventory.itemContainerDestin = null; // 두번째 아이템칸 정리
            }
            else
            { // slot에 Item이 없을 때(아이템을 빈자리에 둘 때)
                item = _inventory.itemContainerOrigin; // Slot에 갖고있던 아이템 배치
                icon.sprite = item.Icon; // Slot 아이콘 변경

                _inventory.itemContainerOrigin = null; // Inventory에서 item 제거

                _inventory.selectedIcon.sprite = emptyIcon; // 커서 이미지 초기화(Sprite)
                _inventory.selectedIcon.transform.localPosition = Vector3.zero; // 커서 이미지 초기화(Position)
                _inventory.isSelect = false; // 커서 이미지 위치 update 비활성화
                Cursor.visible = true; // 커서 가시화

                _del.GetComponent<Button>().interactable = false; // Del Button 비활성화
            }
        }
    }


} // End of Script