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
    [SerializeField] private int FOVRange = 10;
    [SerializeField] public Vector3 FOVDirection;
    [SerializeField] private float delayTime = 0.1f; // inspector 0.1f

    public bool isDrawingFOV = false;

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
    }

    public void DrawGuardFOV()
    {
        Debug.Log("DrawFOV");
        // yield return new WaitForSeconds(delayTime);
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
                        guardController.enabled = false; // To Fix
                        return;
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
        /* FOV 삼각형 기준점 표시(For Debug), set > setFlag > setColor
        FOVtileMap.SetTile(GridPointA, FOVCheck); FOVtileMap.SetTileFlags(GridPointA, TileFlags.None); FOVtileMap.SetColor(GridPointA, Color.red);
        FOVtileMap.SetTile(GridPointB, FOVCheck); FOVtileMap.SetTileFlags(GridPointB, TileFlags.None); FOVtileMap.SetColor(GridPointB, Color.blue);
        DrawFOV = false;
        */
    } // End of DrawGuardFOV()

    /* DrawFOVMesh()
    {
        // float stepAngleSize = Vector3.Angle(LeftBorder, RightBorder) / stepCount;

        // viewPoints.Clear();
        // ViewCastInfo oldViewCast = new ViewCastInfo();
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    FindEdge(oldViewCast, newViewCast, out edgeMinPoint, out edgeMaxPoint);
                    if (edgeMinPoint != Vector3.zero)
                        viewPoints.Add(edgeMinPoint);
                    if (edgeMaxPoint != Vector3.zero)
                        viewPoints.Add(edgeMaxPoint);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.up * maskCutawayDst;

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        }
            */
    /*
*     void DrawFOV_TilePoint()
{
FOVtileMap.SwapTile(FOVCheck, null);
for (int i = 0; i < guardController.grid_view_border.Length; i++)
{
 FOVtileMap.SetTile(guardController.grid_view_border[i], FOVCheck);
 FOVtileMap.SetTileFlags(guardController.grid_view_border[i], TileFlags.None);
 FOVtileMap.SetColor(guardController.grid_view_border[i], Color.black);
 if (i == guardController.grid_view_border.Length / 2)
 {
     FOVtileMap.SetColor(guardController.grid_view_border[i], Color.red);
 }
}
}

IEnumerator DrawFOVWithDelay(float delay)
{
if (isDrawFOV)
{
 yield return new WaitForSeconds(delay);
 StartCoroutine("DrawFOVWithDelay", 0.1f);
}
else DrawFOV();
}
* 
IEnumerator FindTargetsWithDelay(float delay)
{
while (true)
{
 yield return new WaitForSeconds(delay);
 FindVisibleTargets();
}
}
void FindVisibleTargets()
{ // Player 적발 메서드
visibileTargets.Clear();
poly[0] = this.transform.position; // 원점
poly[1] = FOVtileMap.CellToWorld(guardController.grid_view_border[guardController.grid_view_border.Length / 2]); // 중앙
poly[2] = FOVtileMap.CellToWorld(guardController.grid_view_border[0]); // 좌측
poly[3] = FOVtileMap.CellToWorld(guardController.grid_view_border[guardController.grid_view_border.Length - 1]); // 우측

for (int i = 0; i < targetsInViewRadius.Count; i++)
{
 Transform target = targetsInViewRadius[i].transform;
 Vector3 dirToTarget = (target.position - transform.position).normalized;
 if (isInside(poly, target.position))
 {
     float disToTarget = Vector3.Distance(transform.position, target.position);
     if (!Physics2D.Raycast(transform.position, dirToTarget, disToTarget, obstacleMask))
     {
         visibileTargets.Add(target);
     }
 }
}
}

private void OnTriggerEnter2D(Collider2D col)
{
targetsInViewRadius.Add(col);
}
private void OnTriggerExit2D(Collider2D col)
{
targetsInViewRadius.Remove(col);
}
bool isInside(Vector3[] polygon, Vector3 B)
{
//crosses는 점B와 오른쪽 반직선과 다각형과의 교점의 개수
int crosses = 0;
for (int i = 0; i < polygon.Length; i++)
{
 int j = (i + 1) % polygon.Length;
 //점 B가 선분 (p[i], p[j])의 y좌표 사이에 있음
 if ((polygon[i].y > B.y) != (polygon[j].y > B.y))
 {
     //atX는 점 B를 지나는 수평선과 선분 (p[i], p[j])의 교점
     double atX = (polygon[j].x - polygon[i].x) * (B.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x;
     //atX가 오른쪽 반직선과의 교점이 맞으면 교점의 개수를 증가시킨다.
     if (B.x < atX)
         crosses++;
 }
}
return crosses % 2 > 0;
}


void FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, out Vector3 minPoint, out Vector3 maxPoint)
{
float minAngle = minViewCast.angle;
float maxAngle = maxViewCast.angle;
minPoint = Vector3.zero;
maxPoint = Vector3.zero;

for (int i = 0; i < edgeResolveIterations; i++)
{
 float angle = (minAngle + maxAngle) / 2;
 ViewCastInfo newViewCast = ViewCast(angle);

 bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
 if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
 {
     minAngle = angle;
     minPoint = newViewCast.point;
 }
 else
 {
     maxAngle = angle;
     maxPoint = newViewCast.point;
 }
}
} // End of FindEdge()

ViewCastInfo ViewCast(float globalAngle)
{
Vector3 dir = DirFromAngle(globalAngle);
var hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

if (hit)
 return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);

return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
} // End of ViewCast()

public Vector3 DirFromAngle(float angleInDegrees)
{
return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);

} // End of DirFromAngle()

public struct ViewCastInfo
{
public bool hit;
public Vector3 point;
public float dst;
public float angle;

public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
{
hit = _hit;
point = _point;
dst = _dst;
angle = _angle;
}
} // End of Struct ViewCastInfo
private Vector3 AngleToDirZ(float angleInDegree)
{
float radian = (angleInDegree - transform.eulerAngles.z) * Mathf.Deg2Rad;
return new Vector3(Mathf.Sin(radian), Mathf.Cos(radian), 0f);
} // End of AngleToDirZ()

*/

} // End Of Script