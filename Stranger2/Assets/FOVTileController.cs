using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FOVTileController : MonoBehaviour
{
    Tilemap tilemap;
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        // Debug.Log("name : " + col.gameObject.name + ", grid : " + tilemap.WorldToCell(col.transform.position));
    }
}
