using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerRenderer : MonoBehaviour
{
    public Animator anim;

    private GameObject fX_FootStep;
    private ParticleSystem[] SoundEffectors;
    private GameObject S_FX_Collider_Normal;
    private GameObject S_FX_Collider_Small;

    public Camera Cam; // Player Rotation - Mouse Position Check
    public Vector3 mousePos; // Player Rotation - Mouse Position Check
    public Vector3 VecMouseToPlayer;

    public Vector3 playerDirection;

    public Sprite[] wallSprites;
    public Sprite[] wallShadowSprites;

    private GameObject wallCarryingObj; // PlayerController.Combine()에서 참조
    private Transform TF_wallObj;
    private Transform TF_wallShadowObj;
    private Transform TF_wallColliderObj;
    private SpriteRenderer SR_wallObj;
    private SpriteRenderer SR_wallShadowObj;
    private BoxCollider2D BC2D_wallColliderObj;
    Vector3 tmpV3;
    /*
    */
    private PlayerController playerController;

    public int carryWallLayer;
    public bool isSlow = false;
    public bool isCarryWall = false;
    public bool isCombining = false;

    private float playerAngle;

    // public AnimationClip[] animClips;

    private void Awake()
    {
        wallCarryingObj = transform.GetChild(transform.childCount - 3).gameObject;
        TF_wallObj = wallCarryingObj.transform.GetChild(0).GetComponent<Transform>();
        TF_wallShadowObj = wallCarryingObj.transform.GetChild(1).GetComponent<Transform>();
        TF_wallColliderObj = wallCarryingObj.transform.GetChild(2).GetComponent<Transform>();
        SR_wallObj = wallCarryingObj.transform.GetChild(0).GetComponent<SpriteRenderer>();
        SR_wallShadowObj = wallCarryingObj.transform.GetChild(1).GetComponent<SpriteRenderer>();
        BC2D_wallColliderObj = wallCarryingObj.transform.GetChild(2).GetComponent<BoxCollider2D>();

        fX_FootStep = transform.GetChild(transform.childCount - 2).gameObject;
        SoundEffectors = new ParticleSystem[fX_FootStep.transform.childCount - 2];
        for (int i = 0; i < SoundEffectors.Length; i++)
        {
            SoundEffectors[i] = fX_FootStep.transform.GetChild(i).GetComponent<ParticleSystem>();
        }
        S_FX_Collider_Normal = fX_FootStep.transform.GetChild(fX_FootStep.transform.childCount - 2).gameObject;
        S_FX_Collider_Small = fX_FootStep.transform.GetChild(fX_FootStep.transform.childCount - 1).gameObject;
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        playerController = this.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        mousePos = Vector3.zero;
        VecMouseToPlayer = Vector3.zero;
        playerDirection = Vector3.down;
        carryWallLayer = anim.layerCount - 1; // Base : 0, Carray Wall : 1

        S_FX_Collider_Normal.SetActive(false);
        S_FX_Collider_Small.SetActive(false);
        wallCarryingObj.SetActive(false);
        // SetSoundFXEvent("Play_SFX_Small");
    }
    private void Update()
    {
        if (!isCarryWall)
        {
            if (Input.GetKey(KeyCode.Space))
            { // toggle Slow <-> Normal
                isSlow = true;
                anim.SetBool("isSlow", true);
            }
            else
            {
                isSlow = false;
                anim.SetBool("isSlow", false);
            }
        }
        if (S_FX_Collider_Normal.activeSelf || S_FX_Collider_Small.activeSelf)
        {
            StartCoroutine("Is_S_FX_Playing"); // To Fix(최적화)
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
        // playerAngle = GetAngle(playerController.rb.velocity.normalized, playerDirection);
        playerAngle = GetAngle(playerController.tryMove.normalized, playerDirection);
        if (playerController.tryMove == Vector2.zero)
        { // toggle Idle
            anim.SetBool("isWalking", false);
            anim.SetBool("isSideWalking", false);
        }
        else
        {
            if (66f < playerAngle && playerAngle < 76f
                || 85f < playerAngle && playerAngle < 95f
                || 103f < playerAngle && playerAngle < 113f
                || 246f < playerAngle && playerAngle < 256f
                || 265f < playerAngle && playerAngle < 275f
                || 283f < playerAngle && playerAngle < 293f)
            { // toggle SideWalk
                playerController.moveSpeed = playerController.sideWalkSpeed;
                anim.SetBool("isSideWalking", true);
                anim.SetBool("isWalking", false);
            }
            else
            { // toggle Walk(Carrying / Normal)
                if (isSlow)
                { // SlowWalk or Carrying
                    playerController.moveSpeed = playerController.slowWalkSpeed;
                }
                else
                { // Normal Walk
                    playerController.moveSpeed = playerController.walkSpeed;
                }
                anim.SetBool("isWalking", true);
                anim.SetBool("isSideWalking", false);
            }
        }

        if (!isCombining)
        {
            RenderPlayer();
            if (isCarryWall)
            {
                isSlow = true;
                anim.SetBool("isSlow", true);
                RenderPlayerCarryWall();
            }
        }
    }

    /*
    private void SetSoundFXEvent(string FXName)
    {
        animClips = Resources.LoadAll<AnimationClip>("Animation/Clips/Player_Slow_SideWalk");
        float FrameRate = 1 / 60f;
        float[] frameToSet = new float[1];
        // float[] frameToSet = new float[2];
        frameToSet[0] = 18f;
        // frameToSet[1] = 48f;
        AnimationEvent[] animationEvent = new AnimationEvent[1];
        // AnimationEvent[] animationEvent = new AnimationEvent[2];
        for (int i = 0; i < animationEvent.Length; i++)
        {
            animationEvent[i] = new AnimationEvent();
            animationEvent[i].time = frameToSet[i] * FrameRate;
            animationEvent[i].functionName = FXName;

        }
        for (int i = 0; i < animClips.Length; i++)
        {
            AnimationUtility.SetAnimationEvents(animClips[i], animationEvent);
        }
    }
    */

    private IEnumerator Is_S_FX_Playing()
    {
        yield return new WaitForSeconds(1f); // 1f = S_FX_Duration
        if (!(SoundEffectors[0].isPlaying || SoundEffectors[1].isPlaying))
        {
            S_FX_Collider_Normal.SetActive(false);
        }
        if (!(SoundEffectors[2].isPlaying || SoundEffectors[3].isPlaying))
        {
            S_FX_Collider_Small.SetActive(false);
        }
        else
            yield break;
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
        { // child(0:MovementCollider, 1:CarryWallCollider(inactivate), 2:PlayerLegSprite, 3: FootStepEffector / +4:Wall / defaultChildCount = 4
            wallCarryingObj.SetActive(true);
            tmpV3 = Vector3.one;
            float tmpFactor_X = playerDirection.x;
            float tmpFactor_Y = playerDirection.y;

            if (tmpFactor_X == 0)
            { // N & S
                SR_wallObj.sprite = wallSprites[0];
                SR_wallShadowObj.sprite = wallShadowSprites[0];

                tmpV3.x = 1 * tmpFactor_Y;
                TF_wallObj.localScale = tmpV3; // Set(1 * tmpFactor_Y, 1, 1);

                tmpV3.x = -1 * tmpFactor_Y;
                TF_wallShadowObj.localScale = tmpV3; // Set(-1 * tmpFactor_Y, 1, 1);

                tmpV3 = Vector3.zero; tmpV3.y = 0.12f * tmpFactor_Y;
                TF_wallColliderObj.localPosition = tmpV3; // Set(0, 0.12f * tmpFactor_Y, 0);

                TF_wallColliderObj.localEulerAngles = Vector3.zero; // Set(0,0,0);

                tmpV3.x = 0.86f; tmpV3.y = 0.05f;
                BC2D_wallColliderObj.size = tmpV3; // Set(0.86f, 0.05f);

                tmpV3 = Vector3.zero; tmpV3.y = 0.05f + tmpFactor_Y * 0.13f;
                TF_wallObj.localPosition = tmpV3; // Set(0, 0.05f + tmpFactor_Y * 0.13f, 0);

                tmpV3.y = -0.01f + tmpFactor_Y * 0.13f;
                TF_wallShadowObj.localPosition = tmpV3; // Set(0, -0.01f + tmpFactor_Y * 0.13f, 0);
            }
            else if (tmpFactor_Y == 0)
            { // E & W
                SR_wallObj.sprite = wallSprites[1];
                SR_wallShadowObj.sprite = wallShadowSprites[1];

                tmpV3.x = 1 * tmpFactor_X;
                TF_wallObj.localScale = tmpV3; // Set(1 * tmpFactor_X, 1, 1);

                tmpV3.x = 1 * tmpFactor_X;
                TF_wallShadowObj.localScale = tmpV3; // Set(1 * tmpFactor_X, 1, 1);

                tmpV3.x = 0.22f * tmpFactor_X; tmpV3.y = -0.12f; tmpV3.z = 0;
                TF_wallObj.localPosition = tmpV3; // Set(0.22f * tmpFactor_X, -0.12f, 0);

                tmpV3.y = -0.16f;
                TF_wallShadowObj.localPosition = tmpV3; // Set(0.22f * tmpFactor_X, -0.16f, 0);

                tmpV3.y = 0f;
                TF_wallColliderObj.localPosition = tmpV3; // Set(0.22f * tmpFactor_X, 0, 0);

                tmpV3.x = 0; tmpV3.z = 90f;
                TF_wallColliderObj.localEulerAngles = tmpV3; // Set(0, 0, 90);

                tmpV3.x = 0.46f; tmpV3.y = 0.05f; tmpV3.z = 0;
                BC2D_wallColliderObj.size = tmpV3; // Set(0.46f, 0.05f);

            }
            else
            { // NE, NW, SW, SE / else if (tmpFactor_X * tmpFactor_Y == 1 || tmpFactor_X * tmpFactor_Y == -1)
                SR_wallObj.sprite = wallSprites[2];
                SR_wallShadowObj.sprite = wallShadowSprites[2];

                tmpV3.x = 1 * tmpFactor_X * tmpFactor_Y;
                TF_wallObj.localScale = tmpV3; // Set(1 * tmpFactor_X * tmpFactor_Y, 1, 1);
                TF_wallShadowObj.localScale = tmpV3; // Set(1 * tmpFactor_X * tmpFactor_Y, 1, 1);

                tmpV3.x = 0.22f * tmpFactor_X; tmpV3.y = 0.06f * tmpFactor_Y; tmpV3.z = 0;
                TF_wallColliderObj.localPosition = tmpV3; // Set(0.22f * tmpFactor_X, 0.06f * tmpFactor_Y, 0);

                tmpV3 = Vector3.zero; tmpV3.z = -26.565f * tmpFactor_X * tmpFactor_Y;
                TF_wallColliderObj.localEulerAngles = tmpV3; // Set(0, 0, -26.565f * tmpFactor_X * tmpFactor_Y);

                tmpV3.x = 0.72f; tmpV3.y = 0.05f; tmpV3.z = 0;
                BC2D_wallColliderObj.size = tmpV3; // Set(0.72f, 0.05f);

                tmpV3.x = 0.21f * tmpFactor_X; tmpV3.y = 0.04f + tmpFactor_Y * 0.06f;
                TF_wallObj.localPosition = tmpV3; // Set(0.21f * tmpFactor_X, 0.04f + tmpFactor_Y * 0.06f, 0);

                tmpV3.y = 0.01f + tmpFactor_Y * 0.06f;
                TF_wallShadowObj.localPosition = tmpV3; // Set(0.21f * tmpFactor_X, 0.01f + tmpFactor_Y * 0.06f, 0);
            }
        }
    }

    public void Play_SFX_Normal()
    { // SFX_Num = Only 0 or 1
        S_FX_Collider_Normal.SetActive(true);
        if (SoundEffectors[0].isPlaying && SoundEffectors[1].isPlaying)
        { // case1 : Effector 0 & 1 Playing // 수정필요
            // Debug.Log(SoundEffectors[0].time + ", " + SoundEffectors[1].time);
            int i = SoundEffectors[0].time >= SoundEffectors[1].time ? 0 : 1;
            SoundEffectors[i].Stop();
            SoundEffectors[i].Clear();
            SoundEffectors[i].Play();
        }
        else if (!SoundEffectors[0].isPlaying && !SoundEffectors[1].isPlaying)
        { // case2 : Effector 0 & 1 Not Playing
            SoundEffectors[0].Play();
        }
        else
        { // case3 : Only One Effector is Playing(0 or 1)
            int tmp = new int();
            if (SoundEffectors[0].isPlaying)
            {
                tmp = 1;
            }
            else if (SoundEffectors[1].isPlaying)
            {
                tmp = 0;
            }
            SoundEffectors[tmp].Stop();
            SoundEffectors[tmp].Clear();
            SoundEffectors[tmp].Play();
        }
    }
    public void Play_SFX_Small()
    { // SFX_Num = Only 2 or 3
        S_FX_Collider_Small.SetActive(true);
        if (SoundEffectors[2].isPlaying && SoundEffectors[3].isPlaying)
        { // case3 : Effector 2 & 3 Playing
            // Debug.Log(SoundEffectors[2].time + ", " + SoundEffectors[3].time);
            int i = SoundEffectors[2].time >= SoundEffectors[3].time ? 2 : 3;
            SoundEffectors[i].Stop();
            SoundEffectors[i].Clear();
            SoundEffectors[i].Play();
        }
        else if (!SoundEffectors[2].isPlaying && !SoundEffectors[3].isPlaying)
        { // case2 : Effector 2 & 3 Not Playing
            SoundEffectors[2].Play();
        }
        else
        { // case3 : Only One Effector is Playing(2 or 3)
            int tmp = new int();
            if (SoundEffectors[2].isPlaying)
            {
                tmp = 3;
            }
            else if (SoundEffectors[3].isPlaying)
            {
                tmp = 2;
            }
            SoundEffectors[tmp].Stop();
            SoundEffectors[tmp].Clear();
            SoundEffectors[tmp].Play();
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Guard"))
        {
            Debug.Log("누...누구야!");
        }
    }
}