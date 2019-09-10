using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform _playerTranform;
    Transform _camTranform;
    Vector3 _tmpVec;

    private void Awake()
    {
        _playerTranform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _camTranform = GetComponent<Transform>();
        _tmpVec = _camTranform.position;
    }

    private void Update()
    {
        _tmpVec.x = _playerTranform.position.x;
        _tmpVec.y = _playerTranform.position.y;
        _camTranform.position = _tmpVec;
    }
}