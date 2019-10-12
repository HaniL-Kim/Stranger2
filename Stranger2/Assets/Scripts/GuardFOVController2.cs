using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GuardFOVController2 : MonoBehaviour
{
    UIController uIController;
    private Transform tf;
    private GameObject guardObj;
    [SerializeField] private GuardController guardController; // inspector 직접 할당
    [SerializeField] private Tile FOVCheck; // inspector 직접 할당
    public Tilemap FOVtileMap; // inspector 직접 할당
    [SerializeField] private bool isDrawFOV;

    private int stepCount;
    private float range;
    private Vector3[] lineEnds;
    private Ray2D[] ray2Ds;
    private float[] rayDists;
    private RaycastHit2D hit2D;
    private Vector2 checkPoint;
    private int layerMask;
    private float rayDist;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    private void Awake()
    {
        uIController = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UIController>();
        tf = GetComponent<Transform>();
        guardObj = tf.parent.gameObject;
        guardController = guardObj.GetComponentInChildren<GuardController>();
        FOVtileMap = guardObj.GetComponentInChildren<Tilemap>();

        range = guardController.FOVRange;
        stepCount = guardController.FOVRange * 2 + 1;
        lineEnds = new Vector3[stepCount];
        ray2Ds = new Ray2D[stepCount];
        rayDists = new float[stepCount];

        layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Player"));// Wall, Player 체크

        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        rayDist = 0.32f * range;
        GetComponent<EllipseCollider2D>().radiusX = rayDist;
        GetComponent<EllipseCollider2D>().radiusY = rayDist * 0.5f;
    }

    public void InitiateFOV()
    {
        for (int i = 0; i < range+ 1; i++) // guardController.FOVRange
        { // even : (min)0 ~ 40(max)
            lineEnds[2 * i] = FOVtileMap.CellToWorld(guardController.grid_view_border[i]);
            if (i < range)
            { // odd : 1 ~ 39
                lineEnds[2 * i + 1] = 0.5f * (lineEnds[2 * i] + lineEnds[2 * i + 2]);
            }
        }
        for (int i = 0; i < stepCount; i++)
        {
            ray2Ds[i].origin = tf.position;
            ray2Ds[i].direction = lineEnds[i] - tf.position; // normalize automatically
            rayDists[i] = (lineEnds[i] - tf.position).magnitude; // so multyply length
        }
        // DrawFOV();
        DrawFOV2();
    }
    public void DrawFOV2()
    {
        isDrawFOV = true;
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i < stepCount; i++)
        {
            ray2Ds[i].origin = tf.position; // fix1
            // ray2Ds[i].origin = tf.position;
            hit2D = Physics2D.Raycast(tf.position, ray2Ds[i].direction, rayDist, layerMask); // fix1
            // hit2D = Physics2D.Raycast(ray2Ds[i].origin, ray2Ds[i].direction, rayDists[i], layerMask); // collide within mask(Wall, Pillar, Player)
            if (hit2D.collider != null)
            {
                if (hit2D.collider.CompareTag("PlayerTrigger"))
                { // Player에 충돌시
                    guardController.CancelInvoke(); // To Fix
                    uIController.GameOver();
                    break;
                }
                else if (hit2D.collider.CompareTag("WallTrigger") || hit2D.collider.CompareTag("Obstacle"))
                {
                    viewPoints.Add(hit2D.point);
                }
            }
            else
            {
                viewPoints.Add(lineEnds[i]);
            }
        }
        int vertCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[3 * (vertCount - 2)];
        vertices[0] = Vector3.zero; // Local Position;
        for (int i = 0; i < vertCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertCount - 2)
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

    public void DrawFOV()
    { // ToFix Using Multi Thread
      // Debug.Log("DrawFOV()");
        isDrawFOV = true;
        FOVtileMap.SwapTile(FOVCheck, null);

        for (int i = 0; i < stepCount; i++)
        {
            ray2Ds[i].origin = tf.position;
            hit2D = Physics2D.Raycast(ray2Ds[i].origin, ray2Ds[i].direction, rayDists[i], layerMask); // collide within mask(Wall, Pillar, Player)
            for (int j = 1; j < range + 1; j++)
            {
                checkPoint = (j * ((lineEnds[i] - tf.position) / range)) + tf.position;
                if (hit2D.collider != null)
                {

                    if (hit2D.collider.CompareTag("PlayerTrigger"))
                    { // Player에 충돌시
                        FOVtileMap.SwapTile(FOVCheck, null);
                        guardController.CancelInvoke(); // To Fix
                        uIController.GameOver();
                        break;
                    }
                    else if (hit2D.collider.CompareTag("WallTrigger"))
                    { // WallTrigger에 충돌시
                        if (Vector3.Dot((hit2D.point - checkPoint), (lineEnds[i] - tf.position)) < 0)
                        {
                            continue;
                        }
                    }
                }
                FOVtileMap.SetTile(FOVtileMap.WorldToCell(checkPoint), FOVCheck);
                isDrawFOV = false;
            } // For OuterLoop(Ray Count)
        } // For InnerLoop(Check Count)
    } // End Of DrawFOV()

} // End of Script
