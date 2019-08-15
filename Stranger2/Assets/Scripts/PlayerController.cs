using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed; // Player Movement(Inspector Input)
    private Rigidbody2D rb; // Player Movement
    private Vector2 tryMove;


    private PlayerRenderer playerRenderer;
    private Vector2 playerDirection;
    private RaycastHit2D rayClickhit; // Click으로 Wall collider 감지(DeCombine)
    public float rayClickDistance; // Inspector : 0.3f
    private int layerMask;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRenderer = this.GetComponent<PlayerRenderer>();
        layerMask = 1 << LayerMask.NameToLayer("Wall");  // Wall만 충돌 체크
        // layerMask = ~layerMask; 해당 레이어 제외하고 모든 충돌 체크
    }

    void FixedUpdate()
    {
        Vector2 tryMove = Vector2.zero;

        if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0)
        {
            tryMove.x += (Input.GetAxisRaw("Horizontal"));
        }
        if (Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0)
        {
            tryMove.y += (Input.GetAxisRaw("Vertical"));
        }

        rb.velocity = Vector2.ClampMagnitude(tryMove, 1f) * walkSpeed;

        if (Input.GetMouseButtonDown(0))
        {
            PlayerAction();
        }
    }

    /* Side Walk, Front Walk, Back Walk animation 추가시 수정
    if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0)
    if (Input.GetKey(KeyCode.W)) // side walk
        Play sideWalk Anim
    if (Input.GetKey(KeyCode.W || KeyCode.up)
        tryMove += Vector3Int.up;
        Play frontWalk Anim
    if (Input.GetKey(KeyCode.S || KeyCode.down))
        tryMove += Vector3Int.down;
        Play backWalk Anim
    */
    /* space bar 입력시 slowWalk 애니메이션 재생 > walkSpeed 조정
    if (Input.GetKey(KeyCode.Space))
    {
        rb.velocity = Vector3.ClampMagnitude(tryMove, 1f) * slowWalkSpeed;
        return;
    }
    */

    private void PlayerAction()
    {
        playerDirection = playerRenderer.playerDirection;
        rayClickhit = Physics2D.Raycast(this.transform.position, playerDirection, rayClickDistance, layerMask);
        Debug.DrawRay(this.transform.position, playerDirection * rayClickDistance, Color.red, 0.1f); // Click Ray Check

        if (rayClickhit.collider == null)
        {
            if (this.transform.childCount == 3)
            {
                Debug.Log("Action : 맨손으로 무언가 하려 했지만 근처에 아무것도 없습니다.");
                return;
            }
            else if (this.transform.childCount == 4)
            {
                Debug.Log("Action : Wall을 들고 무언가 하려했지만 근처에 아무것도 없습니다.");
                return;
            }
            else return;
        }

        else
        // Debug.Log(rayClickhit.collider.name);
        {
            if (this.transform.childCount == 3)
            { // Not Carrying
                if (rayClickhit.collider.tag == "Wall")
                {
                    DeCombine(rayClickhit.collider.gameObject);
                    return;
                }
                else
                {
                    Debug.Log("Action : [" + rayClickhit.collider.name + "] 에 무언가 시도했지만 아무일도 일어나지 않습니다.");
                    return;
                }
            }
            else if (this.transform.childCount == 4)
            { // Carrying
                if (rayClickhit.collider.tag == "Pillar")
                {
                    Combine(rayClickhit.collider.gameObject);
                    return;
                }
                else if (rayClickhit.collider.tag == "Wall")
                {
                    Debug.Log("Action : Combine() 하려했지만 [" + rayClickhit.collider.name + "] 에 가로막혀 있습니다.");
                    return;
                }
                else
                {
                    Debug.Log("Action : Wall을 들고 [" + rayClickhit.collider.name + "] 에 무언가 시도했지만 아무일도 일어나지 않습니다.");
                    return;
                }
            }
            else return;
        }
    }

    private void DeCombine(GameObject wallObj)
    {
        /*
         * Play DeCombine Animation
         * After DeCombine Animation, SetParent obj to Player
         */
        // play DeCombine Anim
        // Vector2 tmpWallDirection = wallObj.transform.localPosition;
        if (playerDirection.SqrMagnitude() == 1)
        { // Player Direction이 (N,W,S,E)방향일 때
            Debug.Log("DeCombine Error : DeCombine() 하려면 Wall과 마주서야합니다.");
            return;
        }
        else
        { // Player Direction이 (NW,SW,SE,NE)방향일 때
            wallObj.transform.SetParent(this.transform); // if ((tmpWallDirection + playerDirection).SqrMagnitude() < 0.813f)
            Debug.Log("Combine() 했습니다.");
            return;
        }
    }

    private void Combine(GameObject pillarObj)
    {
        /*
         * Play Combine Animation
         * After Combine Animation, SetParent Player to PillarObj
         */
        // play Combine Anim

        if (playerDirection.SqrMagnitude() == 1)
        { // Player 방향이 (NWSE)방향일 때
            Debug.Log("Combine Error : Combine() 하려면 Pillar 정면을 보고 있어야 합니다.");
            return;
        }
        else
        { // Player Direction이 (NW,SW,SE,NE)방향일 때
            playerRenderer.CombinedWallReset(pillarObj);
            Debug.Log("DeCombine() 했습니다.");

        }
    }

}