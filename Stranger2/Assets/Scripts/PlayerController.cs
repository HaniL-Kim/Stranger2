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
        if (rayClickhit.collider != null)
        {
            Debug.Log(rayClickhit.collider.name);
            if (this.gameObject.transform.childCount != 4)
            { // Not Carrying
                if (rayClickhit.collider.tag == "Wall")
                {
                    DeCombine(rayClickhit.collider.gameObject);
                }
                else Debug.Log("DeCombine()하려면 Wall 가까이에 있어야 합니다.");
            }
            else if (this.gameObject.transform.childCount == 4)
            { // Carrying
                if (rayClickhit.collider.tag == "Pillar")
                {
                    Combine(rayClickhit.collider.gameObject);
                }
                else Debug.Log("Combine()하려면 소켓 가까이에 있어야 합니다.");
            }
        }
    }

    private void DeCombine(GameObject wallToDecombine)
    {
        /*
         * Play DeCombine Animation
         * After DeCombine Animation, SetParent obj to Player
         */
        // play DeCombine Anim
        if (this.gameObject.transform.childCount != 4)
        {
            Vector2 tmpWallDirection = wallToDecombine.transform.position;
            tmpWallDirection.Normalize();
            if (playerDirection + tmpWallDirection == Vector2.zero)
            {
                wallToDecombine.gameObject.transform.SetParent(this.transform);
            }
        }
    }

    private void Combine(GameObject pillarToCombine)
    {
        /*
         * Play Combine Animation
         * After Combine Animation, SetParent Player to PillarObj
         */
        // play Combine Anim
        if (this.gameObject.transform.childCount == 4)
        {
            // Pillar에 Combine할 위치에 Wall이 없을 때
            // Name으로 구분? Collider로 구분?
            playerRenderer.wallCarrying.gameObject.transform.SetParent(pillarToCombine.transform);
            playerRenderer.carryWallCollider.SetActive(false);
        }
    }

}