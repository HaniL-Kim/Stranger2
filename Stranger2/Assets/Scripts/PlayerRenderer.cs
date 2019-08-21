using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    public Animator anim;
    private Camera Cam; // Player Rotation - Mouse Position Check
    private Vector2 mousePos; // Player Rotation - Mouse Position Check
    public Vector2 VecMouseToPlayer;

    public Vector2 playerDirection;
    public GameObject wallCarrying; // PlayerController.Cobine()에서 참조

    // private enum playerDirectionEnum { N, NW, W, SW, S, SE, E, NE };
    // private playerDirectionEnum playerDirectionState;
    public Sprite[] wallSprite;

    public Color wallColor; // Player Behind Wall (Caching)

    public GameObject carryWallCollider; // CarryWall (Caching)
    private Quaternion tmpRotation; // CarryWall (Caching)
    private Vector3 tmpVec3carryWall; // CarryWall (Caching)
    private Vector2 tmpWallVec; // CombinedWallReset() Caching

    private PlayerController playerController;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mousePos = Vector2.zero;
        playerDirection = Vector2.down;
        VecMouseToPlayer = Vector2.zero;
        wallColor = Color.white;
        tmpWallVec = Vector2.zero;
        tmpVec3carryWall = Vector3.one;

        playerController = this.GetComponent<PlayerController>();
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

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        { // toggle SlowIdle / Idle
            if (Input.GetKey(KeyCode.Space))
            { // SlowIdle
                anim.SetBool("isSlowIdle", true);
                anim.SetBool("isIdle", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("isSlowWalking", false);
            }
            else
            { // Idle
                anim.SetBool("isIdle", true);
                anim.SetBool("isSlowIdle", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("isSlowWalking", false);
            }
        }
        else
        { // toggle SlowWalk / Walk
            if (Input.GetKey(KeyCode.Space))
            { // SlowWalk
                playerController.moveSpeed = playerController.slowWalkSpeed;
                anim.SetBool("isSlowWalking", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("isIdle", false);
                anim.SetBool("isSlowIdle", false);
            }
            else
            { // Walk
                playerController.moveSpeed = playerController.walkSpeed;
                anim.SetBool("isWalking", true);
                anim.SetBool("isIdle", false);
                anim.SetBool("isSlowIdle", false);
                anim.SetBool("isSlowWalking", false);
            }
        }

        

        // 마우스 위치에 따른 캐릭터 방향 전환(Animation)
        mousePos = Input.mousePosition;
        mousePos = Cam.ScreenToWorldPoint(mousePos);

        VecMouseToPlayer.x = mousePos.x - this.transform.position.x;
        VecMouseToPlayer.y = mousePos.y - this.transform.position.y;
        VecMouseToPlayer.Normalize(); // mouse위치 - player 위상 Vector 정규화

        if (VecMouseToPlayer != null)
        {
            if (Mathf.Cos(Mathf.PI * 3 / 8f) <= VecMouseToPlayer.x)
            {
                playerDirection.x = 1;
            }
            else if (-(Mathf.Cos(Mathf.PI * 3 / 8f)) <= VecMouseToPlayer.x && VecMouseToPlayer.x < Mathf.Cos(Mathf.PI * 3 / 8f))
            {
                playerDirection.x = 0;

            }
            else if (VecMouseToPlayer.x < -(Mathf.Cos(Mathf.PI * 3 / 8f)))
            {
                playerDirection.x = -1;
            }

            if (Mathf.Sin(Mathf.PI / 8f) < VecMouseToPlayer.y)
            {
                playerDirection.y = 1;
            }
            else if (-(Mathf.Sin(Mathf.PI / 8f)) < VecMouseToPlayer.y && VecMouseToPlayer.y < Mathf.Sin(Mathf.PI / 8f))
            {
                playerDirection.y = 0;
            }
            else if (VecMouseToPlayer.y < -(Mathf.Sin(Mathf.PI / 8f)))
            {
                playerDirection.y = -1;
            }
        }
        anim.SetFloat("Direction_X", playerDirection.x);
        anim.SetFloat("Direction_Y", playerDirection.y);
    }


    private void carryWall()
    {
        /*
         * Adjust Wall position & alpha & sprite
         * activate wall collider
         * Play Carry Animation
         */
        if (this.gameObject.transform.childCount == 4)
        { // child(0:movementCollider, 1:seeThroughWallCollider, 2:carryWallCollider(inactivate), 3:Wall(dynamic)
            carryWallCollider = this.transform.GetChild(this.transform.childCount - 2).gameObject;
            carryWallCollider.SetActive(true);
            wallCarrying = this.transform.GetChild(this.transform.childCount - 1).gameObject;
            wallCarrying.GetComponent<Transform>().localPosition = playerDirection / 5; // wallCarrying localPosition

            float tmpFloat = playerDirection.x * playerDirection.y; // wallCarrying localScale
            tmpVec3carryWall.x = tmpFloat;
            if (tmpFloat != 0)
            {
                wallCarrying.GetComponent<Transform>().localScale = tmpVec3carryWall;
            }

            wallCarrying.GetComponent<Collider2D>().enabled = false; // wall edgeCollider disable
            wallColor.a = 128f / 255f; // alpha Change
            wallCarrying.GetComponent<SpriteRenderer>().color = wallColor; // wall Color Change
            wallColor.a = 255f / 255f; // alpha Reset
            if (playerDirection.x == 1 || playerDirection.x == -1)
            {
                wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[2];
                if (playerDirection.x == 1)
                {
                    if (playerDirection.y == 1)
                    { // NE
                        tmpRotation.eulerAngles = new Vector3(0, 0, 155);
                    }
                    if (playerDirection.y == -1)
                    { // SE
                        tmpRotation.eulerAngles = new Vector3(0, 0, 25);
                    }
                }
                else if (playerDirection.x == -1)
                {
                    if (playerDirection.y == 1)
                    { // NW
                        tmpRotation.eulerAngles = new Vector3(0, 0, 205);
                    }
                    if (playerDirection.y == -1)
                    { // SW
                        tmpRotation.eulerAngles = new Vector3(0, 0, 335);
                    }
                }
            }
            else if (playerDirection.x == 0)
            {
                wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[1];
                if (playerDirection.y == 1)
                { // N
                    tmpRotation.eulerAngles = new Vector3(0, 0, 180);
                }
                if (playerDirection.y == 0)
                { // S(Default)
                    tmpRotation.eulerAngles = new Vector3(0, 0, 0);
                }
                if (playerDirection.y == -1)
                { // S
                    tmpRotation.eulerAngles = new Vector3(0, 0, 0);
                }
            }
            if (playerDirection.y == 0)
            {
                wallCarrying.GetComponent<SpriteRenderer>().sprite = wallSprite[0];
                if (playerDirection.x == 1)
                { // E
                    tmpRotation.eulerAngles = new Vector3(0, 0, 90);
                }
                if (playerDirection.x == -1)
                { // W
                    tmpRotation.eulerAngles = new Vector3(0, 0, 270);
                }
            }
            carryWallCollider.transform.rotation = tmpRotation;
        }
    }

    public void CombinedWallReset(GameObject pillarToCombine)
    {
        wallCarrying.transform.SetParent(pillarToCombine.transform);
        tmpWallVec.x = -(playerDirection.x / 2);
        tmpWallVec.y = -(playerDirection.y / 4);
        wallCarrying.transform.localPosition = tmpWallVec;
        wallCarrying.GetComponent<SpriteRenderer>().color = wallColor; // wall Color Change
        wallCarrying.GetComponent<Collider2D>().enabled = true; // wall edgeCollider disable
        wallCarrying = null;
        carryWallCollider.SetActive(false);
    }
}