using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GuardFOVController : MonoBehaviour
{
    /*
    [SerializeField] public bool m_bDebugMode;
    
    [Range(0f, 360f)] [SerializeField] public float m_horizontalViewAngle = 0f;
    [SerializeField] public float m_viewRadius = 1f;
    [Range(-180f, 180f)] [SerializeField] public float m_viewRotateZ = 0f; // rotation값 변경 없이 시야 회전시 사용
    private float m_horizontalViewHalfAngle = 0f;
    */
    [SerializeField] private LayerMask m_viewFOVMask; // FOV LayerMask
    [SerializeField] private LayerMask m_viewTargetMask; // Player LayerMask
    [SerializeField] private LayerMask m_viewObstacleMask; // Wall LayerMask

    private List<Collider2D> hitedTargetContainer = new List<Collider2D>(); // 

    [SerializeField] private Tilemap FOVtileMap;
    [SerializeField] private Tile FOVCheck; // inspector
    [SerializeField] private Vector2Int FOVRange;
    [SerializeField] private Vector2Int FOVDirection;
    [SerializeField] private Vector3Int FOVcurrentCell;
    // [SerializeField] private int countOfRay; // 
    // [SerializeField] private Color TmpColor; // 
    private void Awake()
    {
        FOVtileMap = this.transform.GetComponentInChildren<Tilemap>();
        FOVRange = new Vector2Int(13, 13);
        FOVDirection = new Vector2Int(1, 1);
        FOVcurrentCell = Vector3Int.zero;
        // countOfRay = 16;
        // TmpColor = new Color(1, 1, 1, 1);
    }

    private void FixedUpdate()
    {
        // if Guard Rotate or Move, Tile Refresh
        // this.tilemap.RefreshAllTiles();
        DrawGuardFOV();
        FOVcurrentCell = GameObject.Find("Ground").GetComponent<Tilemap>().WorldToCell(transform.position); // Guard의 위치를 Grid 좌표로
    }

    private Vector3 AngleToDirZ(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
    }

    /*
private void OnDrawGizmos()
{
if (m_bDebugMode)
{
    m_horizontalViewHalfAngle = m_horizontalViewAngle * 0.5f;

    Vector3 originPos = transform.position;
    Gizmos.DrawWireSphere(originPos, m_viewRadius);

    Vector3 horizontalRightDir = AngleToDirZ(-m_horizontalViewHalfAngle + m_viewRotateZ);
    Vector3 horizontalLeftDir = AngleToDirZ(m_horizontalViewHalfAngle + m_viewRotateZ);
    Vector3 lookDir = AngleToDirZ(m_viewRotateZ);
    Vector3 rayDir = Vector3.zero; // To Draw FOV
    Debug.DrawRay(originPos, horizontalLeftDir * m_viewRadius, Color.cyan);
    Debug.DrawRay(originPos, lookDir * m_viewRadius, Color.green);
    Debug.DrawRay(originPos, horizontalRightDir * m_viewRadius, Color.cyan);
    for (int i = 0; i < countOfray; i++)
    {
        if (i == 0)
        {
            rayDir = AngleToDirZ(-m_horizontalViewHalfAngle + m_viewRotateZ + 1f);
        }
        else if (i == countOfray-1)
        {
            rayDir = AngleToDirZ(m_horizontalViewHalfAngle + m_viewRotateZ - 1f);
        }
        else
        {
            rayDir = AngleToDirZ(-m_horizontalViewHalfAngle + m_viewRotateZ + ((m_horizontalViewAngle / (countOfray-1)) * i));
        }
        Debug.DrawRay(originPos, rayDir * m_viewRadius, Color.red);
        int x, y;
        Vector3Int v3Int = new Vector3Int(x, y, 0);
        tileMap.SetTileFlags(v3Int, TileFlags.None);
        tileMap.SetColor(v3Int, (Color.red));
    }
}
}
        */


    private void DrawGuardFOV()
    {
        /* 경비병의 시야
        * [기획의도]
        * 1. 최대 범위는 맵 최대 범위
        * 2. 각도는 방향에 따라 isometric하게 변화
        * 3. 한칸의 크기는 타일크기와 동일
        * 4. 경비병 회전시 시야 방향 전환은 <smooth>

        * ## Renderer.DrawPixel은 시야 표현에 자유도가 낮다.(도트그래픽 Renderer적용 > hard)
        * ## Tile 전체의 Color에 특정 Cell의 Color는 포함된다.(tile Alpha = 0, cell Alpha = 1 > 안보임)
        * ## Alpha만 있는 Sprite는 색상변화가 불가능하다.
        * 
        * [Solution A]
        * 1. Guard의 자식으로 isometricGrid 생성
        * 2. 타일생성
        *   2.1. 스크립트로 'FOVCheck' 타일을 그리드에 동적 배치. > SetTile
        *   2-2. 타일을 미리 배치 해두고 Color만 동적 조정. > GetColor
        * 3. GuardDirection에 따라 8방향 시야 전환(이후 smooth하게 전환하는 과정 추가)
        * 4. 충돌감지
        *   4.1. Player일시 GameOver
        *   4.2. Pillar or Wall 일 시 Oclussion Culling
        *       4-2-a. Collider로 감지
        *       4-2-b. Raycast로 감지
        */
        FOVtileMap.SwapTile(FOVCheck, null);
        /*FOVCheck이 적용된 모든 타일을 null로 만듬
         * TODO : 경비병이 다수 일 때 적용 가능할지?
         */

        Vector3Int TmpFOVPoint = Vector3Int.zero;
        Vector3Int TmpFOVPointCross = Vector3Int.zero;
        int tmpAxix = FOVDirection.x * FOVDirection.y;
        /*
         * tmpAxix == 1 > Facing N or S / N:(1,1), S(-1,-1)
         * tmpAxix == -1 > Facing E or W / W(-1,1), E(1,-1)
         * tmpAxix == 0(x=0) > Facing NW or SE / NW(0,1), SE(0,-1)
         * tmpAxix == 0(y=0) > Facing NE or SW / NE(1,0), SW(-1,0)
         */

        for (int i = 0; i < FOVRange.x; i++)
        {
            // TmpFOVPoint.x = i * tmpAxix;
            for (int j = 0; j < TmpFOVPoint.x; j++)
            {
                // TmpFOVPoint.y = j * tmpAxix;
                TmpFOVPoint.y = Mathf.CeilToInt(TmpFOVPoint.y + (0.5f));
                TmpFOVPointCross.x = TmpFOVPoint.y;
                TmpFOVPointCross.y = TmpFOVPoint.x;

                FOVtileMap.SetTile(TmpFOVPoint, FOVCheck);
                FOVtileMap.SetTile(TmpFOVPointCross, FOVCheck);
            }
            TmpFOVPoint.y = Mathf.CeilToInt(TmpFOVPoint.x - (0.5f));
            TmpFOVPoint.x += tmpAxix;
            /*
             * Algo B(Change Tile Color)
             * TmpColor.a = 1f;
             * tileMap.SetTileFlags(TmpFOVPoint, TileFlags.None);
             * FOVtileMap.SetColor((TmpFOVPoint, TmpColor);
             * this.tilemap.RefreshAllTiles();
            */
        }
    }
}