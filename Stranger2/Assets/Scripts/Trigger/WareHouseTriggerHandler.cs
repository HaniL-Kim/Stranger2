using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WareHouseTriggerHandler : MonoBehaviour
{
    public GameObject WareHouse;
    public GameObject WareHouse_Roof;
    public GameObject WareHouse_Pillar;
    public SpriteRenderer[] SR_WareHouse_Roof_Arr;
    public SpriteRenderer[] SR_WareHouse_Pillar_Arr;
    public List<SpriteRenderer> SR_ObjAbove_List;
    public List<SpriteRenderer> SR_ObjBelow_List;

    private void Awake()
    {
        WareHouse = this.transform.parent.gameObject;
        WareHouse_Roof = WareHouse.transform.GetChild(0).gameObject;
        WareHouse_Pillar = WareHouse.transform.GetChild(1).gameObject;
        SR_WareHouse_Roof_Arr = WareHouse_Roof.GetComponentsInChildren<SpriteRenderer>();
        SR_WareHouse_Pillar_Arr = WareHouse_Pillar.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < SR_WareHouse_Pillar_Arr.Length; i++)
        {
            if(SR_WareHouse_Pillar_Arr[i].transform.parent.CompareTag("ObjBelow"))
            {
                SR_ObjBelow_List.Add(SR_WareHouse_Pillar_Arr[i]);
            }
            else if(SR_WareHouse_Pillar_Arr[i].transform.parent.CompareTag("ObjAbove"))
            {
                SR_ObjAbove_List.Add(SR_WareHouse_Pillar_Arr[i]);
            }
        }
        GameObject[] pillarType0Arr = GameObject.FindGameObjectsWithTag("PillarType0");
        for (int i = 0; i < pillarType0Arr.Length; i++)
        { // Type0 Pillar's Wall inActivate
            pillarType0Arr[i].transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    /* TODO
     * 1. 콜라이더 별 스크립트 작성
     * 2. 콜라이더별 오브젝트 & 스크립트
     * 3. fade In, Out 메서드 매개변수 설정, 코드 재사용
     */
}
