using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Del : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Sprite emptyIcon;

    void Awake()
    {
        _inventory = FindObjectOfType<Inventory>();
        GetComponent<Button>().interactable = false;
    }

    public void DeleteItem()
    {
        if(_inventory.itemContainerOrigin != null)
        {
            _inventory.itemContainerOrigin = null;
            _inventory.selectedIcon.sprite = emptyIcon; // 커서 이미지 초기화(Sprite)
            _inventory.selectedIcon.transform.localPosition = Vector3.zero; // 커서 이미지 초기화(Position)
            _inventory.isSelect = false; // 커서 이미지 위치 update 비활성화
            Cursor.visible = true; // 커서 가시화

            GetComponent<Button>().interactable = false; // Del Button 비활성화
        }
        
    }
}
