﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    [SerializeField] public float moveSpeed; // Player Movement
    [SerializeField] public float walkSpeed; // Player Movement(Inspector Input)
    [SerializeField] public float slowWalkSpeed; // Player Movement(Inspector Input)
    [SerializeField] public float sideWalkSpeed; // Player Movement(Inspector Input)
    [SerializeField] private float rayClickDistance = 0.3f; // Inspector : 0.3f
    [SerializeField] private float linearDrag = 0.3f; // Inspector : 0.3f

    public Rigidbody2D rb; // Player Movement
    public Vector2 tryMove;
    private PlayerRenderer playerRenderer;
    private Vector3 playerDirection;
    private RaycastHit2D rayClickhit; // Click으로 Wall collider 감지(DeCombine)
    private GameObject objByHit;
    private int layerMask;

    public float angerGauge = 0f;
    public float imPatience = 1f;

    public int defaultChildCount;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRenderer = this.GetComponent<PlayerRenderer>();
        layerMask = 1 << LayerMask.NameToLayer("Pillar");// Pillar 만 체크 / layerMask = ~layerMask; 해당 레이어 제외
        // layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Pillar"));// Wall, Pillar 만 체크 / layerMask = ~layerMask; 해당 레이어 제외
        defaultChildCount = this.transform.childCount;
    }

    void Update()
    {
        if (!GameController.pauseOn)
        { // pauseOn == false 일 때
            if (!EventSystem.current.IsPointerOverGameObject())
            { // 포인터가 UI 위에 있지 않을 때
                if (Input.GetMouseButtonDown(0))
                {
                    PlayerAction();
                }
            }
        }
    }

    void FixedUpdate()
    {
        tryMove.x = Input.GetAxis("Horizontal") * 2f;
        tryMove.y = Input.GetAxis("Vertical");
        rb.AddForce(tryMove * moveSpeed * Time.deltaTime);
        if (tryMove.x == 0 && tryMove.y == 0)
        {
            if (Mathf.Abs(rb.velocity.x) < linearDrag && Mathf.Abs(rb.velocity.y) < linearDrag)
            {
                rb.velocity = Vector2.zero;
            }
        }

        /*
        Vector2 tryMove = Vector2.zero;
        if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0)
        {
            tryMove.x += (Input.GetAxisRaw("Horizontal"));
        }
        if (Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0)
        {
            tryMove.y += (Input.GetAxisRaw("Vertical"));
        }
        */
        //rb.velocity = Vector2.ClampMagnitude(tryMove, 1f) * moveSpeed;
    }

    private void PlayerAction()
    {
        playerDirection = playerRenderer.playerDirection;
        string rayTargetName = SetRayTarget();
        bool isCarryWall = playerRenderer.isCarryWall;
        if (!isCarryWall)
        { // 맨손 일 때
            rayClickhit = Physics2D.Raycast(this.transform.position, playerDirection, rayClickDistance, layerMask); // 몸에서부터 ray 시작
            Debug.DrawRay(this.transform.position, playerDirection * rayClickDistance, Color.blue, 0.5f); // 디버그레이 파란색
            StartCombineAfterPillarCheck(rayTargetName, isCarryWall);
        }
        else if (isCarryWall)
        { // 벽을 들고 있을 때
            rayClickhit = Physics2D.Raycast(this.transform.position + (playerDirection.normalized * 0.1f), playerDirection, rayClickDistance * 0.3f, layerMask); // 벽들고 있을땐 벽에서부터 ray, 거리 1/3
            Debug.DrawRay(this.transform.position + (playerDirection.normalized * 0.1f), playerDirection * rayClickDistance * 0.3f, Color.red, 0.5f); // 벽들고 있을땐 빨간색
            StartCombineAfterPillarCheck(rayTargetName, isCarryWall);
        }
    } // End of PlayerAction()

    public string SetRayTarget()
    {
        string result = null;
        if (playerDirection.x == 1)
        {
            if (playerDirection.y == 1)
            {
                result = "SW";
            }
            else if (playerDirection.y == -1)
            {
                result = "NW";
            }
        }
        else if (playerDirection.x == -1)
        {
            if (playerDirection.y == 1)
            {
                result = "SE";
            }
            else if (playerDirection.y == -1)
            {
                result = "NE";
            }
        }
        return result;
    }

    public void StartCombineAfterPillarCheck(string rayTargetName, bool isCarryWall)
    {
        if (rayClickhit.collider == null)
        {
            Debug.Log("Action : 맨손으로 무언가 하려 했지만 근처에 아무것도 없습니다.");
            return;
        }
        else
        {
            if (rayClickhit.collider.CompareTag("PillarTrigger"))
            { // 
                objByHit = rayClickhit.collider.gameObject;
                bool isWallActive = rayClickhit.transform.parent.GetChild(0).gameObject.activeSelf;
                if ( (isWallActive && !isCarryWall) || (!isWallActive && isCarryWall) )
                { // 맨손 + Pillar에 Wall이 활성화 or 벽들고 + Pillar에 Wall 비활성
                    if (Vector3.SqrMagnitude(playerDirection) != 1)
                    { // Player Direction이 대각선 방향 일 때 (NW,SE,SW,SE)
                        if (rayClickhit.collider.name.EndsWith(rayTargetName))
                        { // Ray 방향과 Collider 방향이 같을 때(rayTargetName = "SE" <-> Collider"SE")
                            playerRenderer.anim.SetTrigger("Combine");
                            StartCoroutine("CombineEvent");
                            return;
                        }
                        else
                        { // Pillar 반대편에 있을 때
                            Debug.Log("CombineEvent Error : 정면에 서야합니다.");
                            return;
                        }
                    }
                    else
                    {  // Player Direction이 수직일 때 (E, W, S, N)
                        Debug.Log("CombineEvent Error : 평행하게 서야합니다.");
                        return;
                    }
                }
                else
                { // Pillar에 Wall이 없을 때
                    if(isWallActive && isCarryWall)
                    {
                        Debug.Log("CombineEvent Error : 벽이 이미 설치되어 있습니다.");
                        return;
                    }
                    else if (!isWallActive && !isCarryWall)
                    {
                        Debug.Log("CombineEvent Error : 해제 할 벽이 없습니다.");
                        return;
                    }
                }
            }
            else
            {// tag가 PillarCollider가 아닐 때
                Debug.Log("Action : [" + rayClickhit.collider.name + "] 에 무언가 시도했지만 아무일도 일어나지 않습니다.");
                return;
            }
        }
    }

    private IEnumerator CombineEvent()
    { // used by event from AnimationClip
        playerRenderer.Play_SFX_Normal();
        bool wallIsActive = objByHit.transform.parent.GetChild(0).gameObject.activeSelf;
        int layerWeight = wallIsActive ? 1 : 0;
        playerRenderer.isCombining = true;
        yield return new WaitForSeconds(0.4f); // 24frame 대기
        bool wallActiveToggle = objByHit.transform.parent.GetChild(0).gameObject.activeSelf ? false : true;
        objByHit.transform.parent.GetChild(0).gameObject.SetActive(wallActiveToggle);
        this.transform.GetChild(1).gameObject.SetActive(!wallActiveToggle);
        playerRenderer.isCarryWall = wallIsActive;
        playerRenderer.anim.SetLayerWeight(playerRenderer.carryWallLayer, layerWeight);
        playerRenderer.isCombining = false;
        if (wallIsActive)
        {
            Debug.Log("DeCombine() 했습니다.");
        }
        else
        {
            Debug.Log("Combine() 했습니다.");
        }
        yield break;
    } // End of CombineEvent()

} // End of Script