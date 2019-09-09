using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    private GuardFOVController guardFOVController;
    public bool guardRotatePatternOn;
    public float RotatePatternSpeed = 0.05f;
    public float delayTime = 0.15f;

    void Awake()
    {
        guardFOVController = this.GetComponentInChildren<GuardFOVController>();
        guardRotatePatternOn = true;
        StartCoroutine("guardRotatePattern");
    }

    void FixedUpdate()
    {

        /*
        if (guardRotatePatternOn)
        {
            GuardRotatePattern();
        }
        */

    }

    IEnumerator guardRotatePattern()
    {
        // Debug.Log("guardRotatePattern On");
        yield return new WaitForSeconds(delayTime);
        Vector3 normalizedFOVDir = guardFOVController.FOVDirection.normalized;
        Vector3 normalVecOfDir = new Vector3(guardFOVController.FOVDirection.y, -guardFOVController.FOVDirection.x, 0); // 방향 벡터의 법선벡터(right)
        guardFOVController.FOVDirection = Vector3.Lerp(normalizedFOVDir, (normalizedFOVDir + normalVecOfDir).normalized, RotatePatternSpeed);
        StartCoroutine("guardRotatePattern");
    }
    /*
    void GuardRotatePattern()
    {
        Vector3 normalizedFOVDir = guardFOVController.FOVDirection.normalized;
        Vector3 normalVecOfDir = new Vector3(guardFOVController.FOVDirection.y, -guardFOVController.FOVDirection.x, 0); // 방향 벡터의 법선벡터(right)

        guardFOVController.FOVDirection = Vector3.Lerp( normalizedFOVDir, (normalizedFOVDir + normalVecOfDir).normalized, RotatePatternSpeed * Time.deltaTime );
    }
    */

}