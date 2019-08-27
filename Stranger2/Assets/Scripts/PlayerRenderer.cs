using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    public Animator anim;
    public Camera Cam; // Player Rotation - Mouse Position Check
    public Vector3 mousePos; // Player Rotation - Mouse Position Check
    public Vector3 VecMouseToPlayer;

    public Vector3 playerDirection;
    public GameObject wallCarrying; // PlayerController.Combine()에서 참조

    // private enum playerDirectionEnum { N, NW, W, SW, S, SE, E, NE };
    // private playerDirectionEnum playerDirectionState;
    public Sprite[] wallSprite;

    public Color wallColor; // Player Behind Wall (Caching)

    public GameObject carryWallCollider; // CarryWall (Caching)
    private Quaternion tmpRotation; // CarryWall (Caching)
    private Vector3 tmpVec3carryWall; // CarryWall (Caching)
    private Vector3 tmpWallVec; // CombinedWallReset() Caching

    private PlayerController playerController;

    public bool isSlow = false;
    public bool isCarryWall = false;
    public int carryWallLayer;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mousePos = Vector3.zero;
        playerDirection = Vector3.down;
        VecMouseToPlayer = Vector3.zero;
        wallColor = Color.white;
        tmpWallVec = Vector3.zero;
        tmpVec3carryWall = Vector3.one;

        playerController = this.GetComponent<PlayerController>();
        carryWallLayer = anim.layerCount - 1; // Base : 0, Carray Wall : 1
    }
    private void Update()
    {
        if (!isCarryWall)
        {
            if (Input.GetKey(KeyCode.Space))
            { // toggle Slow <-> Normal
                isSlow = true;
                anim.SetBool("isSlow", true);
                return;
            }
            else
            {
                isSlow = false;
                anim.SetBool("isSlow", false);
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        // 키입력에 따른 애니메이션 전환
        /* 
         * 1. toggle Idle
         * 2. toggle SideWalk
         * 3. toggle Walk
         */

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        { // toggle Idle
            anim.SetBool("isWalking", false);
            anim.SetBool("isSideWalking", false);
        }

        else if (GetAngle(playerController.rb.velocity.normalized, playerDirection) == 90f
            || GetAngle(playerController.rb.velocity.normalized, playerDirection) == 270f
            ) // 캐릭터의 이동방향과 바라보는 방향이 수직일 때 / TODO : 차이점 확인 (이동방향 1.RigidBody(적용중) vs 2.Input GetKey)
        { // toggle SideWalk
            { // SideWalk
                playerController.moveSpeed = playerController.sideWalkSpeed;
                anim.SetBool("isSideWalking", true);
                anim.SetBool("isWalking", false);
            }
        }
        else
        { // toggle Walk(방향키 입력만 있을 때(Carrying / Normal)
            if (isSlow)
            { // SlowWalk(Carrying)
                playerController.moveSpeed = playerController.slowWalkSpeed;
                anim.SetBool("isWalking", true);
                anim.SetBool("isSideWalking", false);
            }
            else
            { // Walk(Normal)
                playerController.moveSpeed = playerController.walkSpeed;
                anim.SetBool("isWalking", true);
                anim.SetBool("isSideWalking", false);
            }
        }

        RenderPlayer();
        if (isCarryWall)
        {
            isSlow = true;
            anim.SetBool("isSlow", true);
            RenderPlayerCarryWall();
            return;
        }
    }

    private void RenderPlayer()
    {
        // 마우스 위치에 따른 애니메이션 전환
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


    private void RenderPlayerCarryWall()
    {
        /*
         * Adjust Wall position & alpha & sprite
         * activate wall collider
         * Play Carry Animation
         */
        if (isCarryWall)
        { // child(0:movementCollider, 1:seeThroughWallCollider, 2:carryWallCollider(inactivate), 3:PlayerLegSprite, 4:Wall
            carryWallCollider = this.transform.GetChild(this.transform.childCount - 3).gameObject; // get carryWallCollider Obj & activation
            carryWallCollider.SetActive(true);
            wallCarrying = this.transform.GetChild(this.transform.childCount - 1).gameObject; // get Wall Obj & positioning
            wallCarrying.GetComponent<Transform>().localPosition = playerDirection / 5;

            float tmpFloat = playerDirection.x * playerDirection.y; // wall localScale adj(wall's Direction)
            tmpVec3carryWall.x = tmpFloat;
            if (tmpFloat != 0)
            {
                wallCarrying.GetComponent<Transform>().localScale = tmpVec3carryWall;
            }

            wallCarrying.GetComponent<Collider2D>().enabled = false; // wall's edgeCollider disable
            wallColor.a = 50f / 255f; // alpha Change
            wallCarrying.GetComponent<SpriteRenderer>().color = wallColor; // wall's Color Change
            wallColor.a = 255f / 255f; // alpha Reset
            if (playerDirection.x == 1 || playerDirection.x == -1)
            { // Player Direction에 따른 Wall Sprite 변경
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
        if (wallCarrying != null)
        {
            wallCarrying.transform.SetParent(pillarToCombine.transform);
            tmpWallVec.x = -(playerDirection.x * 0.48f);
            tmpWallVec.y = -(playerDirection.y * 0.24f);
            wallCarrying.transform.localPosition = tmpWallVec;
            wallCarrying.GetComponent<SpriteRenderer>().color = wallColor; // wall Color Change
            wallCarrying.GetComponent<Collider2D>().enabled = true; // wall edgeCollider disable
            wallCarrying = null;
            carryWallCollider.SetActive(false);
        }
    }
    public float GetAngle(Vector3 vec1, Vector3 vec2)
    {
        float theta = Vector3.Dot(vec1, vec2) / (vec1.magnitude * vec2.magnitude);
        Vector3 dirAngle = Vector3.Cross(vec1, vec2);
        float angle = Mathf.Acos(theta) * Mathf.Rad2Deg;
        if (dirAngle.z < 0.0f) angle = 360 - angle;
        // Debug.Log("사잇각 : " + angle);
        return angle;
    }
}