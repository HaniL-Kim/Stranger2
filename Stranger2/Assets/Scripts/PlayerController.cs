using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed;  // Inspector 입력

    private Rigidbody2D rb; // Player Movement

    private Color wallColor; // 투명화 캐싱(플레이어가 벽 후면쪽 이동시 벽 컬러값)
    private Vector2 tempPos; // 투명화 캐싱 플레이어가 벽 후면쪽 이동시 벽 위치값)



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        wallColor = new Color(1, 1, 1, 1);
        tempPos = new Vector2(0, 0);
    }

    void FixedUpdate()
    {
        // 캐릭터 이동
        Vector3 tryMove = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0)
        {
            tryMove.x += (Input.GetAxisRaw("Horizontal"));
        }
        if (Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0)
        {
            tryMove.y += (Input.GetAxisRaw("Vertical"));
        }
        /*
        if (Input.GetKey(KeyCode.A)) // side walk
            tryMove += Vector3Int.left;
        if (Input.GetKey(KeyCode.D)) // side walk
            tryMove += Vector3Int.right;
        if (Input.GetKey(KeyCode.W))
            tryMove += Vector3Int.up;
        if (Input.GetKey(KeyCode.S))
            tryMove += Vector3Int.down;
        */
        /* spacebar 입력시 slowWalk 애니메이션, walkSpeed 조정
        if (Input.GetKey(KeyCode.Space))
        {
            rb.velocity = Vector3.ClampMagnitude(tryMove, 1f) * slowWalkSpeed;
            return;
        }
        */
        rb.velocity = Vector3.ClampMagnitude(tryMove, 1f) * walkSpeed;
    }

    public static float CalculateAngle(Vector2 from, Vector2 to)
    { // 두 벡터 사이 각도 구하기(0 ~ 360)
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }


    void OnTriggerExit2D(Collider2D col)
    { // 근접한 pillar or wall 투명화 해제
        col.gameObject.GetComponent<SpriteRenderer>().color = wallColor; // collider alpha 초기화
    }

    void OnTriggerEnter2D(Collider2D col)
    { // 근접한 pillar or wall 투명화
        if (col.tag == "Pillar" || col.tag == "Wall") // collider 태그 확인
        { 
            // todo : back wall은 투명도 조절x
            tempPos = col.gameObject.GetComponent<Transform>().position; // collider 좌표 획득
            if (this.transform.position.y > tempPos.y) // Player <-> Collider y좌표 비교
            { // collider y좌표가 player y좌표보다 낮은값 일 때
                Debug.Log("TriggenEnter From Back");
                wallColor.a = 128f / 255f; // 캐시 alpha 값 반투명 설정
                col.gameObject.GetComponent<SpriteRenderer>().color = wallColor; // collider alpha 수정
                wallColor.a = 255f / 255f; // 캐시 alpha 값 초기화
            }
        }
    }
}
