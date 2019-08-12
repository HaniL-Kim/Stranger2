using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed; // Player Movement(Inspector Input)
    private Rigidbody2D rb; // Player Movement


    private PlayerRenderer playerRenderer;
    private Vector2 playerDirection;
    private RaycastHit2D rayClickhit; // Click으로 Wall collider 감지(DeCombine)
    public float rayClickDistance; // Inspector : 0.3f
    public LayerMask layerMask;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRenderer = this.GetComponent<PlayerRenderer>();
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerDirection = playerRenderer.playerDirection;
            /*
            playerDirection.x = playerRenderer.h_move;
            playerDirection.y = playerRenderer.v_move;
            */
            rayClickhit = Physics2D.Raycast(this.transform.position, playerDirection, rayClickDistance, layerMask);
            Debug.DrawRay(this.transform.position, playerDirection * rayClickDistance, Color.red, 0.1f); // Click Ray Check
            if(rayClickhit.collider != null)
            {
                if (rayClickhit.collider.tag == "Wall")
                {
                    DeCombine(rayClickhit.collider.gameObject);
                    Debug.Log(rayClickhit.collider.name);
                }
            }

        }

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

        /* Side Walk 추가시 수정
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

    private void DeCombine(GameObject obj)
    {
        /*
         * Play DeCombine Animation
         * After DeCombine Animation, SetParent obj to Player
         */
         // player DeCombine Anim
        // if(들고있지 않을때)
        obj.gameObject.transform.SetParent(this.transform);
    }
}