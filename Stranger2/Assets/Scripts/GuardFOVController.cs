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
    [SerializeField] private Tile FOVCheck = null; // inspector
    [SerializeField] private bool ClearFOV = true; // inspector
    [SerializeField] private bool DrawFOV = false; // inspector
    [SerializeField] private int FOVRange;
    [SerializeField] private Vector3 FOVDirection;
    [SerializeField] private Vector3Int FOVcurrentCell;

    public bool debugMode;

    private int layerMask;
    Vector3[] Vec3ForGizmo;
    // [SerializeField] private Color TmpColor; // 
    private void Awake()
    {
        FOVtileMap = this.transform.GetComponentInChildren<Tilemap>();
        FOVRange = 10;
        FOVDirection = new Vector3(1, 1, 0);
        FOVcurrentCell = Vector3Int.zero;

        layerMask = 1 << LayerMask.NameToLayer("Wall");  // Wall만 충돌 체크
        Vec3ForGizmo = new Vector3[FOVRange * 2 + 1];
        // TmpColor = new Color(1, 1, 1, 1);
    }

    private void FixedUpdate()
    {
        // if Guard Rotate or Move, Tile Refresh
        // this.tilemap.RefreshAllTiles();
        if (DrawFOV)
        {
            DrawGuardFOV();
        }

        FOVcurrentCell = GameObject.Find("Ground").GetComponent<Tilemap>().WorldToCell(transform.position); // Guard의 위치를 Grid 좌표로
    }

    private Vector3 AngleToDirZ(float angleInDegree)
    {
        float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
    }

    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            for (int i = 0; i < Vec3ForGizmo.Length; i++)
            {
                if (i == 0)
                    Debug.DrawRay(this.transform.position, Vec3ForGizmo[i], Color.red);
                else if (i == Vec3ForGizmo.Length - 1)
                    Debug.DrawRay(this.transform.position, Vec3ForGizmo[i], Color.blue);
                else
                    Debug.DrawRay(this.transform.position, Vec3ForGizmo[i], Color.green);

            }
        }
    }

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
    private void DrawGuardFOV()
    {
        Debug.Log("DrawFOV");
        if (ClearFOV)
        {
            FOVtileMap.SwapTile(FOVCheck, null);
        }
        /*FOVCheck이 적용된 모든 타일을 null로 만듬
         * TODO : 경비병이 다수 일 때 적용 가능할지?
         */

        Vector3Int TmpFOVPoint = Vector3Int.zero;
        Vector3 pointA, pointB, normalVecOfDir;
        Vector3[] rayDirections = new Vector3[FOVRange * 2 + 1];
        RaycastHit2D[] rays = new RaycastHit2D[rayDirections.Length];

        int halfFOVRange = FOVRange / 2;

        Vector3 FOVDirectionNormalize = FOVDirection.normalized; // 방향 벡터
        normalVecOfDir = new Vector3(FOVDirectionNormalize.y, -FOVDirectionNormalize.x, 0); // 방향 벡터의 법선벡터(right)
        pointA = this.transform.position + (FOVDirectionNormalize * FOVRange + (normalVecOfDir * Mathf.Tan(0.25f * Mathf.PI)) * halfFOVRange); // 내위치 + (방향벡터 * range) + (법선벡터(right) * halfRange)
        pointB = this.transform.position + (FOVDirectionNormalize * FOVRange - (normalVecOfDir * Mathf.Tan(0.25f * Mathf.PI)) * halfFOVRange); // 내위치 + (방향벡터 * range) + (법선벡터(left) * halfRange)

        Vector3Int GridPointA = FOVtileMap.WorldToCell(pointA);
        Vector3Int GridPointB = FOVtileMap.WorldToCell(pointB);

        for (int i = 0; i < rayDirections.Length; i++) // Direction of Ray
        { // (rayDirections.Length -1) means : origin = pointB, destination = pointA
            Vector3 pointAVec = pointA - this.transform.position;
            Vector3 pointBVec = pointB - this.transform.position;
            rayDirections[i] = (i * ((pointBVec - pointAVec) / (rayDirections.Length - 1))) + pointAVec; // OP(n) = ( n/20 * (AB) ) + OA
            rays[i] = Physics2D.Raycast(this.transform.position, rayDirections[i], rayDirections[i].magnitude, layerMask);
            // if (rays[i].collider.gameObject.layer == LayerMask.NameToLayer("Wall"))

            for (int j = 0; j < rayDirections.Length * 2; j++)
            { // 1 ray당 checkPoint는 ray 개수의 2배
                Vector3 checkPoint = this.transform.position + (rayDirections[i] * ((float)j / ((float)rayDirections.Length * (float)2)));
                if (rays[i].collider != null)
                { // wall에 ray가 충돌시
                    Vector3 TmpVec3 = new Vector3((rays[i].point.x - checkPoint.x), (rays[i].point.y - checkPoint.y), 0); // checkPoint와 충돌점 사이의 벡터를 확인하여
                    if (Vector3.Dot(TmpVec3, rayDirections[i]) < 0)
                    { // 벡터의 방향이 ray 방향과 반대일때(내적값이 -) 반복문 빠져나감
                        continue;
                    }
                }
                TmpFOVPoint = FOVtileMap.WorldToCell(checkPoint);
                FOVtileMap.SetTile(TmpFOVPoint, FOVCheck);
            }
            Vec3ForGizmo[i] = rayDirections[i]; // to Debug
        }

        // * FOV 삼각형 기준점 표시(Debug), set > setFlag > setColor
        FOVtileMap.SetTile(GridPointA, FOVCheck); FOVtileMap.SetTileFlags(GridPointA, TileFlags.None); FOVtileMap.SetColor(GridPointA, Color.red);
        FOVtileMap.SetTile(GridPointB, FOVCheck); FOVtileMap.SetTileFlags(GridPointB, TileFlags.None); FOVtileMap.SetColor(GridPointB, Color.blue);
        DrawFOV = false;
    } // End of DrawGuardFOV()

    private bool IsPointInFigure(Vector3Int pointToCheck, Vector3Int[] points)
    {
        bool result = false;
        int crosses = 0;
        for (int i = 0; i < points.Length; i++)
        {
            int j = (i + 1) % points.Length;
            if ((points[i].y > pointToCheck.y) != (points[j].y > pointToCheck.y))
            { // pointToCheck의 y좌표가 선분 (points[i], points[j])의 y좌표 사이에 있음
                double atX = (points[j].x - points[i].x) * (pointToCheck.y - points[i].y) / (points[j].y - points[i].y) + points[i].x; //atX는 점 pointToCheck를 지나는 수평선과 선분 (points[i], points[j])의 교점의 x좌표
                if (pointToCheck.x < atX)
                { // atX가 pointToCheck.x보다 오른쪽(+값)이면 교점의 갯수 추가
                    crosses++;
                }
            }
        }
        result = crosses % 2 > 0; // 교점의 개수가 짝수면 true = inside
        return result;
    }

    private bool IsPointInTriangle(Vector3 pointToCheck, Vector3 pointA, Vector3 pointB)
    {
        /*
         * PointP = pointToCheck, PointC = zero
         * PA x PC, PC x PB, PB x PA (x는 외적)을 각각 구해서 세식의 값이 모두 양수(0제외)이거나 모두 음수이면 삼각형 내부로 판단.
         */
        bool result = false;
        Vector3 pointC, pointP;
        float Cross1, Cross2, Cross3;
        pointC = this.transform.position;
        pointP = pointToCheck;
        Cross1 = Vector3.Cross(pointP - pointA, pointP - pointC).z;
        Cross2 = Vector3.Cross(pointP - pointC, pointP - pointB).z;
        Cross3 = Vector3.Cross(pointP - pointB, pointP - pointA).z;
        if ((Cross1 > 0 && Cross2 > 0 && Cross3 > 0) || (Cross1 < 0 && Cross2 < 0 && Cross3 < 0))
        {
            result = true;
        }
        return result;
    }

    private Vector3 AxialSymmetryPoint(Vector3 pointToSym, Vector3 axis)
    { // axis : ax + by + c = 0
        Vector3 point = Vector3.zero;
        float x1, y1, x2, y2; // 매개변수점과 반환점
        float a, b; // 대칭축의 성분
        x1 = pointToSym.x; x2 = point.x;
        y1 = pointToSym.y; y2 = point.y;
        a = axis.y;
        b = -axis.x;

        x2 = x1 - (2f * a * (a * x1 + b * y1) / (a * a + b * b));
        y2 = y1 - (2f * b * (a * x1 + b * y1) / (a * a + b * b));

        point.x = x2;
        point.y = y2;

        return point;
        // End of AxialSymmetryPoint()
    }

    // End Of Script
}
// TmpFOVPoint.y = j * tmpAxix;
/*Change
TmpFOVPoint.y = Mathf.CeilToInt(TmpFOVPoint.y + (0.5f * FOVDirection.y));
TmpFOVPointCross.x = TmpFOVPoint.y;
TmpFOVPointCross.y = TmpFOVPoint.x;

FOVtileMap.SetTile(TmpFOVPoint, FOVCheck);
FOVtileMap.SetTile(TmpFOVPointCross, FOVCheck);
}
TmpFOVPoint.y = Mathf.CeilToInt(TmpFOVPoint.x - (0.5f * FOVDirection.x));
TmpFOVPoint.x += FOVDirection.x;
/*
/*
* Algo B(Change Tile Color)
* TmpColor.a = 1f;
* tileMap.SetTileFlags(TmpFOVPoint, TileFlags.None);
* FOVtileMap.SetColor((TmpFOVPoint, TmpColor);
* this.tilemap.RefreshAllTiles();
*/
