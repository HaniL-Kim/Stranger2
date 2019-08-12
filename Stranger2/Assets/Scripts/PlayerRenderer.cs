using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    private Animator anim;
    private Camera Cam; // Player Rotation - Mouse Position Check
    private Vector2 mousePos; // Player Rotation - Mouse Position Check
    public Vector2 VecMouseToPlayer;

    private float h_move;
    private float v_move;
    public Vector2 playerDirection;
    private GameObject wallCarrying;

    private enum playerDirectionEnum { N, NW, W, SW, S, SE, E, NE };
    private playerDirectionEnum playerDirectionState;
    public Sprite[] wallSprite;

    private Color wallColor; // Player Behind Wall (Caching)
    private Vector2 tempPos; // Player Behind Wall (Caching)

    private GameObject carryWallCollider; // CarryWall (Caching)
    private Quaternion tmpRotation; // CarryWall (Caching)

    private void Awake()
    {
        anim = GetComponent<Animator>();
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mousePos = new Vector2(0, 0);
        VecMouseToPlayer = new Vector2(0, 0);
        h_move = 0f;
        v_move = 0f;

        wallColor = new Color(1, 1, 1, 1);
        tempPos = new Vector2(0, 0);
    }

    private void FixedUpdate()
    {
        renderPlayerDirection();
        if (this.transform.childCount == 4)
        {
            carryWall();
        }
    }

    private void renderPlayerDirection()
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
            playerDirectionState = playerDirectionEnum.E;
        }
        else if (22.5f <= TmpAngle && TmpAngle < 67.5f)
        { // PlayerIdle NE 애니메이션 재생
            h_move = 1f;
            v_move = 1f;
            playerDirectionState = playerDirectionEnum.NE;
        }
        else if (67.5f <= TmpAngle && TmpAngle < 112.5f)
        { // Static N 애니메이션 재생
            h_move = 0f;
            v_move = 1f;
            playerDirectionState = playerDirectionEnum.N;
        }
        else if (112.5f <= TmpAngle && TmpAngle < 157.5f)
        { // Static NW 애니메이션 재생
            h_move = -1f;
            v_move = 1f;
            playerDirectionState = playerDirectionEnum.NW;
        }
        else if (157.5f <= TmpAngle && TmpAngle < 202.5f)
        { // Static W 애니메이션 재생
            h_move = -1f;
            v_move = 0f;
            playerDirectionState = playerDirectionEnum.W;
        }
        else if (202.5f <= TmpAngle && TmpAngle < 247.5f)
        { // Static SW 애니메이션 재생
            h_move = -1f;
            v_move = -1f;
            playerDirectionState = playerDirectionEnum.SW;
        }
        else if (247.5f <= TmpAngle && TmpAngle < 292.5f)
        { // Static S 애니메이션 재생
            h_move = 0f;
            v_move = -1f;
            playerDirectionState = playerDirectionEnum.S;
        }
        else if (292.5f <= TmpAngle && TmpAngle < 337.5f)
        { // Static SE 애니메이션 재생
            h_move = 1f;
            v_move = -1f;
            playerDirectionState = playerDirectionEnum.SE;
        }
        anim.SetFloat("Direction_X", h_move);
        anim.SetFloat("Direction_Y", v_move);

        playerDirection.x = h_move;
        playerDirection.y = v_move;
    }


    private void carryWall()
    {
        /*
         * Adjust Wall position & alpha & sprite
         * activate wall collider
         * Play Carry Animation
         */
        if (this.gameObject.transform.childCount == 4)
        { // Child(3) = Wall
            carryWallCollider = this.transform.GetChild(2).gameObject;
            carryWallCollider.SetActive(true);
            wallCarrying = this.transform.GetChild(3).gameObject;
            wallCarrying.GetComponent<Transform>().localPosition = playerDirection / 5; // wall positioning
            wallCarrying.GetComponent<Collider2D>().enabled = false; // wall edgeCollider disable
            wallColor.a = 128f / 255f; // 캐시 alpha 값 반투명 설정
            wallCarrying.GetComponent<SpriteRenderer>().color = wallColor;
            wallColor.a = 255f / 255f; // 캐시 alpha 값 초기화
            switch (playerDirectionState)
            {
                case playerDirectionEnum.N:
                    tmpRotation.eulerAngles = new Vector3(0, 0, 180);
                    carryWallCollider.transform.rotation = tmpRotation;
                    wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[1];
                    wallCarrying.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case playerDirectionEnum.S:
                    tmpRotation.eulerAngles = new Vector3(0, 0, 0);
                    carryWallCollider.transform.rotation = tmpRotation;
                    wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[1];
                    wallCarrying.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case playerDirectionEnum.NW:
                    tmpRotation.eulerAngles = new Vector3(0, 0, 205);
                    carryWallCollider.transform.rotation = tmpRotation;
                    wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[2];
                    wallCarrying.GetComponent<SpriteRenderer>().flipX = true;
                    break;
                case playerDirectionEnum.SE:
                    tmpRotation.eulerAngles = new Vector3(0, 0, 25);
                    carryWallCollider.transform.rotation = tmpRotation;
                    wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[2];
                    wallCarrying.GetComponent<SpriteRenderer>().flipX = true;
                    break;
                case playerDirectionEnum.SW:
                    tmpRotation.eulerAngles = new Vector3(0, 0, 335);
                    carryWallCollider.transform.rotation = tmpRotation;
                    wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[2];
                    wallCarrying.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case playerDirectionEnum.NE:
                    tmpRotation.eulerAngles = new Vector3(0, 0, 155);
                    carryWallCollider.transform.rotation = tmpRotation;
                    wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[2];
                    wallCarrying.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case playerDirectionEnum.W:
                    tmpRotation.eulerAngles = new Vector3(0, 0, 270);
                    carryWallCollider.transform.rotation = tmpRotation;
                    wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[0];
                    wallCarrying.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                case playerDirectionEnum.E:
                    tmpRotation.eulerAngles = new Vector3(0, 0, 90);
                    carryWallCollider.transform.rotation = tmpRotation;
                    wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[0];
                    wallCarrying.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Pillar" || col.tag == "Wall") // obj 태그 확인
        {
            tempPos = col.GetComponent<Transform>().position; // obj 좌표 획득
            if (this.transform.position.y - tempPos.y > 0.01f)
            { // obj의 y좌표가 player의 y좌표보다 클 때
                wallColor.a = 128f / 255f; // 캐시 alpha 값 반투명 설정
                col.gameObject.GetComponent<SpriteRenderer>().color = wallColor; // obj alpha 수정
                wallColor.a = 255f / 255f; // 캐시 alpha 값 초기화
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        col.gameObject.GetComponent<SpriteRenderer>().color = wallColor; // obj alpha 수정
    }

}
