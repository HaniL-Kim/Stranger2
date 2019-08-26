using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThrough : MonoBehaviour

{
    private Vector2 tempPos; // Player Behind Wall (Caching)
    private Color wallColor; // Player Behind Wall (Caching)

    private void Awake()
    {
        tempPos = Vector2.zero;
        wallColor = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Pillar" || col.tag == "Wall") // obj 태그 확인
        {
            tempPos = col.GetComponent<Transform>().position; // obj 좌표 획득
            if (this.transform.position.y - tempPos.y > 0.01f)
            { // obj의 y좌표가 player.wallSeeThroughCollider의 y좌표보다 클 때
                wallColor.a = 128f / 255f; // 캐시 alpha 값 반투명 설정
                col.gameObject.GetComponent<SpriteRenderer>().color = wallColor; // obj alpha 수정
                wallColor.a = 255f / 255f; // 캐시 alpha 값 초기화
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Pillar" || col.tag == "Wall") // obj 태그 확인
        {
            col.gameObject.GetComponent<SpriteRenderer>().color = wallColor; // obj alpha 수정
        }
    }
}
