using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardFOVController2 : MonoBehaviour
{
    public float viewRadius; // 3.2f
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask obstacleMask;
    public MeshFilter viewMeshFilter;

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;
    public float maskCutawayDst = .1f;

    Mesh viewMesh;
    List<Vector3> viewPoints = new List<Vector3>();
    Vector3 edgeMinPoint, edgeMaxPoint;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    void LateUpdate()
    {//draw field of view
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        viewPoints.Clear();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = -transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
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
        }

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
    } // End of LateUpdate()


    private void OnDrawGizmos()
    {
        Vector3 originPos = transform.position;

        Gizmos.DrawWireSphere(originPos, viewRadius);

        Vector3 horizontalRightDir = AngleToDirZ(-viewAngle * 0.5f);
        Vector3 horizontalLeftDir = AngleToDirZ(viewAngle * 0.5f);
        Vector3 lookDir = AngleToDirZ(0f);

        Debug.DrawRay(originPos, horizontalLeftDir * viewRadius, Color.cyan);
        Debug.DrawRay(originPos, lookDir * viewRadius, Color.green);
        Debug.DrawRay(originPos, horizontalRightDir * viewRadius, Color.cyan);
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


} // End of Script
