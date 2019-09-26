using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GuardFOVController2 : MonoBehaviour
{
    private Transform tf;
    private GameObject guardObj;
    [SerializeField] private GuardController guardController; // inspector 직접 할당
    [SerializeField] private Tile FOVCheck; // inspector 직접 할당
    public Tilemap FOVtileMap; // inspector 직접 할당
    [SerializeField] private bool isDrawFOV;

    private int stepCount;
    private float range;
    private Vector3[] lineEnd;
    RaycastHit2D hit2D;
    Ray2D ray2D;
    Vector2 checkPoint;
    private int layerMask;

    private void Awake()
    {
        tf = GetComponent<Transform>();
        guardObj = tf.parent.gameObject;
        guardController = guardObj.GetComponentInChildren<GuardController>();
        FOVtileMap = guardObj.GetComponentInChildren<Tilemap>();

        range = guardController.FOVRange;
        stepCount = guardController.FOVRange * 2 + 1;
        lineEnd = new Vector3[stepCount];

        layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Pillar")) + (1 << LayerMask.NameToLayer("Player"));// Wall, Pillar, Player 체크
    }

    public void DrawFOV()
    { // ToFix Using Multi Thread
        // Debug.Log("DrawFOV()");
        isDrawFOV = true;
        FOVtileMap.SwapTile(FOVCheck, null);

        for (int i = 0; i < range + 1; i++) // guardController.FOVRange
        { // even : (min)0 ~ 40(max)
            lineEnd[2 * i] = FOVtileMap.CellToWorld(guardController.grid_view_border[i]);
            if (i < range)
            { // odd : 1 ~ 39
                lineEnd[2 * i + 1] = 0.5f * (lineEnd[2 * i] + lineEnd[2 * i + 2]);
            }
        }

        for (int i = 0; i < stepCount; i++)
        {
            ray2D.origin = tf.position;
            ray2D.direction = lineEnd[i] - tf.position; // normalize automatically
            float rayDist = (lineEnd[i] - tf.position).magnitude; // so multyply length
            hit2D = Physics2D.Raycast(ray2D.origin, ray2D.direction, rayDist, layerMask); // collide within mask(Wall, Pillar, Player)
            // hit2D = Physics2D.Raycast(tf.position, lineEnd[i] - tf.position, (lineEnd[i] - tf.position).magnitude, layerMask);
            // Debug.DrawRay(ray2D.origin, lineEnd[i] - tf.position, Color.white, 0.01f); ;
            for (int j = 1; j < range + 1; j++)
            {
                checkPoint = (j * ((lineEnd[i] - tf.position) / range)) + tf.position;
                if (hit2D.collider != null)
                {
                    if (hit2D.collider.CompareTag("Player"))
                    { // Player에 충돌시
                        FOVtileMap.SwapTile(FOVCheck, null);
                        guardController.CancelInvoke(); // To Fix
                        break;
                    }
                    else
                    { //  if (hit2D.collider.CompareTag("Wall") || hit2D.collider.CompareTag("Pillar"))
                        if (Vector3.Dot((hit2D.point - checkPoint), (lineEnd[i] - tf.position)) < 0)
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
