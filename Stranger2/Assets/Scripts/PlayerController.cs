using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed; // Player Movement
    public float walkSpeed; // Player Movement(Inspector Input)
    public float slowWalkSpeed; // Player Movement(Inspector Input)
    private Rigidbody2D rb; // Player Movement
    private Vector2 tryMove;


    private PlayerRenderer playerRenderer;
    private Vector3 playerDirection;
    private RaycastHit2D rayClickhit; // Click으로 Wall collider 감지(DeCombine)
    public float rayClickDistance; // Inspector : 0.3f
    private int layerMask;

    private GameObject objByHit;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRenderer = this.GetComponent<PlayerRenderer>();
        layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Pillar"));// Wall, Pillar 만 체크

        // layerMask = ~layerMask; 해당 레이어 제외하고 모든 충돌 체크
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayerAction();
        }

        Vector2 tryMove = Vector2.zero;

        if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0)
        {
            tryMove.x += (Input.GetAxisRaw("Horizontal"));
        }
        if (Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0)
        {
            tryMove.y += (Input.GetAxisRaw("Vertical"));
        }

        rb.velocity = Vector2.ClampMagnitude(tryMove, 1f) * moveSpeed;
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
        if (this.transform.childCount == 4)
        {
            rayClickhit = Physics2D.Raycast(this.transform.position, playerDirection, rayClickDistance, layerMask); // 맨손일땐 몸에서부터 ray

            Debug.DrawRay(this.transform.position, playerDirection * rayClickDistance, Color.blue, 0.5f); // 맨손일땐 파란색
            if (rayClickhit.collider == null)
            {
                Debug.Log("Action : 맨손으로 무언가 하려 했지만 근처에 아무것도 없습니다.");
                return;
            }
            else
            {
                if (rayClickhit.collider.tag == "Wall")
                { // tag가 wall 일 때
                    objByHit = rayClickhit.collider.gameObject;
                    if (Vector3.SqrMagnitude(playerDirection) != 1)
                    { // Player Direction : NW,SE,SW,SE
                        playerRenderer.anim.SetTrigger("Combine");
                        return;
                    }
                    else  // Player Direction : N,W,S,E
                        Debug.Log("CombineEvent Error : CombineEvent() 하려면 Wall과 마주서야합니다.");
                    return;
                }
                else
                {// tag가 wall 이 아닐 때
                    Debug.Log("Action : [" + rayClickhit.collider.name + "] 에 무언가 시도했지만 아무일도 일어나지 않습니다.");
                    return;
                }
            }
        }
        else if (this.transform.childCount == 5)
        {
            rayClickhit = Physics2D.Raycast(this.transform.position + (playerDirection.normalized * 0.5f), playerDirection, rayClickDistance*0.3f, layerMask); // 벽들고 있을땐 벽에서부터 ray, 거리 1/3
            Debug.DrawRay(this.transform.position + (playerDirection.normalized * 0.5f), playerDirection * rayClickDistance * 0.3f, Color.red, 0.5f); // 벽들고 있을땐 빨간색
            if (rayClickhit.collider == null)
            {
                Debug.Log("Action : Wall을 들고 무언가 하려했지만 근처에 아무것도 없습니다.");
                return;
            }
            else
            {
                if (rayClickhit.collider.tag == "Pillar")
                { // tag가 Pillar 일 때
                    objByHit = rayClickhit.collider.gameObject;
                    if (Vector3.SqrMagnitude(playerDirection) != 1)
                    { // Player Direction : NW,SE,SW,SE
                        playerRenderer.anim.SetTrigger("Combine");
                        return;
                    }
                    else
                    {
                        Debug.Log("CombineEvent Error : CombineEvent() 하려면 Wall과 마주서야합니다.");
                        return;
                    }
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
        }

    }

    private void CombineEvent()
    { // used by event from AnimationClip


        if (objByHit.tag == "Wall")
        {
            objByHit.transform.SetParent(this.transform); // if ((tmpWallDirection + playerDirection).SqrMagnitude() < 0.813f)
            Debug.Log("DeCombine() 했습니다.");
            return;
        }
        else if (objByHit.tag == "Pillar")
        {
            playerRenderer.CombinedWallReset(objByHit); // animation clip에 event로 적용!
            Debug.Log("Combine() 했습니다.");
            return;
        }
    }

}