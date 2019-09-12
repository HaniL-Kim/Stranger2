using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GuardFOVController : MonoBehaviour
{
    // private List<Collider2D> hitedTargetContainer = new List<Collider2D>();
    [SerializeField] private int layerMask; // LayerMask(Wall, Pillar, Player 체크)
    [SerializeField] private Tilemap FOVtileMap;
    [SerializeField] private Tile FOVCheck = null; // inspector
    [SerializeField] private bool ClearFOV = true; // inspector
    [SerializeField] private int FOVRange;
    [SerializeField] public Vector3 FOVDirection;
    [SerializeField] private float delayTime = 0.1f; // inspector 0.1f

    public bool isDrawingFOV;

    Vector3Int TmpFOVPoint = Vector3Int.zero;
    Vector3 FOVDirNormalize, FOVDirNormalizeTmp, checkPoint, TmpVecForCheckP, pointAVec, pointA, pointBVec, pointB, normalVecTmp, normalVec = Vector3.zero;
    [SerializeField] private Vector3[] rayDirections;
    RaycastHit2D[] rays;
    GuardController guardController;

    private void Awake()
    {
        FOVtileMap = this.transform.GetComponentInChildren<Tilemap>();
        FOVDirection = Vector3.up;

        layerMask = (1 << LayerMask.NameToLayer("Wall"))
            + (1 << LayerMask.NameToLayer("Pillar"))
            + (1 << LayerMask.NameToLayer("Player"));// Wall, Pillar, Player 체크

        rayDirections = new Vector3[FOVRange * 6 + 1];
        rays = new RaycastHit2D[rayDirections.Length];
        guardController = transform.parent.GetComponent<GuardController>();

        StartCoroutine(DrawGuardFOV(delayTime));
    }

    IEnumerator DrawGuardFOV(float delayTime)
    {
        // Debug.Log("DrawFOV");
        yield return new WaitForSeconds(delayTime);
        isDrawingFOV = true;

        if (ClearFOV)
        {
            FOVtileMap.SwapTile(FOVCheck, null);
        }

        int halfFOVRange = FOVRange / 2;
        FOVDirNormalize = FOVDirection.normalized; // 방향 벡터
        FOVDirNormalizeTmp = FOVDirNormalize * FOVRange; // 방향 벡터의 길이보정
        normalVec.x = FOVDirNormalize.y; // 방향 벡터의 법선벡터(right)
        normalVec.y = -FOVDirNormalize.x; // 방향 벡터의 법선벡터(right)
        normalVecTmp = (normalVec * Mathf.Tan(0.25f * Mathf.PI)) * halfFOVRange; // 법선벡터의 길이 보정
        pointA = this.transform.position + (FOVDirNormalizeTmp + normalVecTmp); // 내위치 + (방향벡터 * range) + (법선벡터(right) * halfRange)
        pointB = this.transform.position + (FOVDirNormalizeTmp - normalVecTmp); // 내위치 + (방향벡터 * range) + (법선벡터(left) * halfRange)

        // Vector3Int GridPointA = FOVtileMap.WorldToCell(pointA); 꼭지점 표시(For Debug)
        // Vector3Int GridPointB = FOVtileMap.WorldToCell(pointB); 꼭지점 표시(For Debug)

        for (int i = 0; i < rayDirections.Length; i++) // Direction of Ray
        { // (rayDirections.Length -1) means : origin = pointB, destination = pointA
            pointAVec = pointA - this.transform.position;
            pointBVec = pointB - this.transform.position;
            rayDirections[i] = (i * ((pointBVec - pointAVec) / (rayDirections.Length - 1))) + pointAVec; // OP(n) = ( n/20 * (AB) ) + OA
            rays[i] = Physics2D.Raycast(this.transform.position, rayDirections[i], rayDirections[i].magnitude, layerMask);

            for (int j = 0; j < rayDirections.Length * 2; j++)
            { // 1 ray당 checkPoint는 ray 개수의 2배
                checkPoint = this.transform.position + (rayDirections[i] * ((float)j / ((float)rayDirections.Length * (float)2)));
                if (rays[i].collider != null)
                {
                    if (rays[i].collider.CompareTag("Player"))
                    { // Player에 충돌시
                        Debug.Log("어.어어... 거기 누구야!");
                        FOVtileMap.SwapTile(FOVCheck, null);
                        guardController.StopAllCoroutines();
                        StopAllCoroutines();
                        yield break;
                    }

                    else
                    { // wall에 충돌시
                        TmpVecForCheckP = new Vector3((rays[i].point.x - checkPoint.x), (rays[i].point.y - checkPoint.y), 0); // checkPoint와 충돌점 사이의 벡터를 확인하여
                        if (Vector3.Dot(TmpVecForCheckP, rayDirections[i]) < 0)
                        { // 벡터의 방향이 ray 방향과 반대일때(내적값이 -) 반복문 빠져나감
                            continue;
                        }
                    }
                }
                TmpFOVPoint = FOVtileMap.WorldToCell(checkPoint);
                FOVtileMap.SetTile(TmpFOVPoint, FOVCheck);
            }
            // Vec3ForGizmo[i] = rayDirections[i]; // For Debug
        }
        isDrawingFOV = false;
        StartCoroutine(DrawGuardFOV(delayTime));

        /* FOV 삼각형 기준점 표시(For Debug), set > setFlag > setColor
        FOVtileMap.SetTile(GridPointA, FOVCheck); FOVtileMap.SetTileFlags(GridPointA, TileFlags.None); FOVtileMap.SetColor(GridPointA, Color.red);
        FOVtileMap.SetTile(GridPointB, FOVCheck); FOVtileMap.SetTileFlags(GridPointB, TileFlags.None); FOVtileMap.SetColor(GridPointB, Color.blue);
        DrawFOV = false;
        */
    } // End of DrawGuardFOV()
} // End Of Script