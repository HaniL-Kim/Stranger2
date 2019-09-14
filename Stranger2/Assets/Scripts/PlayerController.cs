using System.Collections;
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
        layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Pillar"));// Wall, Pillar 만 체크 / layerMask = ~layerMask; 해당 레이어 제외
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
        tryMove.x = Input.GetAxis("Horizontal") *2f;
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
        if (this.transform.childCount == defaultChildCount)
        { // 맨손 일 때
            rayClickhit = Physics2D.Raycast(this.transform.position, playerDirection, rayClickDistance, layerMask); // 몸에서부터 ray 시작

            Debug.DrawRay(this.transform.position, playerDirection * rayClickDistance, Color.blue, 0.5f); // 디버그레이 파란색
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
                        StartCoroutine("CombineEvent");
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
        else if (this.transform.childCount == defaultChildCount+1)
        {
            rayClickhit = Physics2D.Raycast(this.transform.position + (playerDirection.normalized * 0.5f), playerDirection, rayClickDistance * 0.3f, layerMask); // 벽들고 있을땐 벽에서부터 ray, 거리 1/3
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
                        StartCoroutine("CombineEvent");
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
    } // End of PlayerAction()

    private IEnumerator CombineEvent()
    { // used by event from AnimationClip
        playerRenderer.Play_SFX_Normal();
        if (objByHit.tag == "Wall")
        {
            playerRenderer.isCombining = true;
            yield return new WaitForSeconds(0.4f); // 24frame 대기
            objByHit.transform.SetParent(this.transform); // if ((tmpWallDirection + playerDirection).SqrMagnitude() < 0.813f)
            playerRenderer.isCarryWall = true;
            playerRenderer.anim.SetLayerWeight(playerRenderer.carryWallLayer, 1);
            playerRenderer.isCombining = false;
            Debug.Log("DeCombine() 했습니다.");
            yield break;
        }
        else if (objByHit.tag == "Pillar")
        {
            playerRenderer.isCombining = true;
            Vector3 combineDir = playerDirection;
            yield return new WaitForSeconds(0.4f); // 24frame 대기
            playerRenderer.CombinedWallReset(objByHit, combineDir); // animation clip에 event로 적용!
            playerRenderer.isCarryWall = false;
            playerRenderer.anim.SetLayerWeight(playerRenderer.carryWallLayer, 0);
            Debug.Log("Combine() 했습니다.");
            yield break;
        }
    } // End of CombineEvent()

} // End of Script