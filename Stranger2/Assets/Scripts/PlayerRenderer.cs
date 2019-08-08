using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    private Animator anim;
    private Camera Cam; // Player Rotation - Mouse Position Check
    private Vector2 mousePos; // Player Rotation - Mouse Position Check
    private Vector2 VecMouseToPlayer;
    private float h_move;
    private float v_move;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mousePos = new Vector2(0, 0);
        VecMouseToPlayer = new Vector2(0, 0);
        h_move = 0f;
        v_move = 0f;
    }

    private void FixedUpdate()
    {
        // 마우스 위치에 따른 캐릭터 방향 전환
        mousePos = Input.mousePosition;
        mousePos = Cam.ScreenToWorldPoint(mousePos);

        VecMouseToPlayer.x = mousePos.x - this.transform.position.x;
        VecMouseToPlayer.y = mousePos.y - this.transform.position.y;
        VecMouseToPlayer.Normalize(); // mouse위치 - player 위상 Vector 정규화

        float TmpAngle = Quaternion.FromToRotation(Vector3.right, VecMouseToPlayer).eulerAngles.z;
        // Debug.Log("위상" + VecMouseToPlayer + "사이각" + TmpAngle);
        // 두 벡터 사이 각에 따라 player방향 결정

        if (TmpAngle < 22.5f || 337.5f <= TmpAngle)
        { // PlayerIdle E 애니메이션 재생
            h_move = 1f;
            v_move = 0f;
            Debug.Log("E");
        }
        else if (22.5f <= TmpAngle && TmpAngle < 67.5f)
        { // PlayerIdle NE 애니메이션 재생
            h_move = 1f;
            v_move = 1f;
            Debug.Log("NE");
        }
        else if (67.5f <= TmpAngle && TmpAngle < 112.5f)
        { // Static N 애니메이션 재생
            h_move = 0f;
            v_move = 1f;
            Debug.Log("N");
        }
        else if (112.5f <= TmpAngle && TmpAngle < 157.5f)
        { // Static NW 애니메이션 재생
            h_move = -1f;
            v_move = 1f;
            Debug.Log("NW");
        }
        else if (157.5f <= TmpAngle && TmpAngle < 202.5f)
        { // Static W 애니메이션 재생
            h_move = -1f;
            v_move = 0f;
            Debug.Log("W");
        }
        else if (202.5f <= TmpAngle && TmpAngle < 247.5f)
        { // Static SW 애니메이션 재생
            h_move = -1f;
            v_move = -1f;
            Debug.Log("SW");
        }
        else if (247.5f <= TmpAngle && TmpAngle < 292.5f)
        { // Static S 애니메이션 재생
            h_move = 0f;
            v_move = -1f;
            Debug.Log("S");
        }
        else if (292.5f <= TmpAngle && TmpAngle < 337.5f)
        { // Static SE 애니메이션 재생
            h_move = 1f;
            v_move = -1f;
            Debug.Log("SE");
        }
        anim.SetFloat("Direction_X", h_move);
        anim.SetFloat("Direction_Y", v_move);
    }
}
