using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    private GuardFOVController2 guardFOVController2;
    public int FOVRange = 0; // Default : 20
    public int FOV_halfWidth = 0; // Default : FOVRange/2, min : 1, max = FOVRange*4
    Dictionary<int, GridInfo> FOV_gridMap;
    [Range(0, 159)]
    public int FOV_gridMap_Key;
    public Vector3Int grid_view_center = Vector3Int.zero;
    public Vector3Int[] grid_view_border;
    [Range(0, 1)]
    public float rotatePatternSpeed = 1;

    // public bool guardRotatePatternOn;
    // public int delayTime = 60;
    // public int delayTimeCounter = 0;

    void Awake()
    {
        guardFOVController2 = this.GetComponentInChildren<GuardFOVController2>();

        FOV_gridMap = new Dictionary<int, GridInfo>();
        StoreGridEdgeInfo();

        FOV_gridMap_Key = 0;
        grid_view_border = new Vector3Int[FOV_halfWidth * 2 + 1];
        InvokeRepeating("guardRotatePattern", 0.1f, rotatePatternSpeed);
    }

        /*
    private void Update()
    {
        if (Time.frameCount % 50 == 0)
        {
            System.GC.Collect();
        }
    }
    void FixedUpdate()
    {
        if (delayTimeCounter == delayTime)
        {
            guardRotatePattern();
        }
        else
        {
            delayTimeCounter++;
        }
    }
    IEnumerator guardRotateWithDelay(float delay)
    {
        if (guardFOVController2.isDrawFOV)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine("guardRotateWithDelay", 0.1f);
        }
        else
            guardRotatePattern(); 
    }
    */
    public void guardRotatePattern()
    {
        if (FOV_gridMap_Key < FOV_gridMap.Count)
        {
            FOV_gridMap_Key++;
        }
        else
        {
            FOV_gridMap_Key = 0;
        }

        GridInfo tmp = new GridInfo();
        FOV_gridMap.TryGetValue(FOV_gridMap_Key, out tmp);
        grid_view_center.x = tmp._coord_X;
        grid_view_center.y = tmp._coord_Y;
        GetGridBorder();
        // delayTimeCounter = 0;
        guardFOVController2.DrawFOV();
    }

    void StoreGridEdgeInfo()
    {
        for (int i = 0; i < (2 * FOVRange) * 4; i++)
        { // Range Default : 20
            if (i < FOVRange * 2) // key : [0] ~ [39]
            { // [0] = (19, 20), [19] = (0, 20), [20] = (1, 20), [39] = (-20, 20)
                FOV_gridMap.Add(i, new GridInfo(FOVRange - i - 1, FOVRange));
            }
            else if (i < FOVRange * 4) // key : [40] ~ [79]
            { // [40] = (-20, 19), [59] = (-20, 0), [60] = (-20, -1), [79] = (-20, -20)
                FOV_gridMap.Add(i, new GridInfo(-FOVRange, FOVRange - i + (2 * FOVRange) - 1));
            }
            else if (i < FOVRange * 6) // [80] ~ [119]
            { // [80] = (-19, -20), [99] = (0, -20), [100] = (1, -20), [119] = (20, -20)
                FOV_gridMap.Add(i, new GridInfo(-(FOVRange - i + (4 * FOVRange) - 1), -FOVRange));
            }
            else if (i < FOVRange * 8) // [120] ~ [159]
            { // [120] = (20, -19), [139] = (20, 0), [140] = (20, 1), [159] = (20, 20)
                FOV_gridMap.Add(i, new GridInfo(FOVRange, -(FOVRange - i + (6 * FOVRange) - 1)));
            }
        }
    }

    void GetGridBorder()
    {
        int[] keys = new int[FOV_halfWidth * 2 + 1];
        GridInfo[] infos = new GridInfo[FOV_halfWidth * 2 + 1];

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = FOV_gridMap_Key - FOV_halfWidth + i;
            if (keys[i] > FOV_gridMap.Count)
            {
                keys[i] = keys[i] - FOV_gridMap.Count - 1;
            }
            else if (keys[i] < 0)
            {
                keys[i] = keys[i] + FOV_gridMap.Count + 1;
            }
            FOV_gridMap.TryGetValue(keys[i], out infos[i]);
            grid_view_border[i].x = infos[i]._coord_X;
            grid_view_border[i].y = infos[i]._coord_Y;
        }
    }

    public struct GridInfo
    {
        public int _coord_X, _coord_Y;
        public GridInfo(int _coord_X, int _coord_Y)
        {
            this._coord_X = _coord_X;
            this._coord_Y = _coord_Y;
        }
        public void ShowGridInfo()
        {
            Debug.Log("_coord_X : " + _coord_X + ", _coord_Y : " + _coord_Y);
            return;
        }
    }
}