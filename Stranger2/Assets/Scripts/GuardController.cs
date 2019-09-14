using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    private GuardFOVController guardFOVController;
    public bool guardRotatePatternOn;
    public float RotatePatternSpeed = 0.05f;
    public int delayTime = 60;
    public int delayTimeCounter = 0;

    void Awake()
    {
        guardFOVController = this.GetComponentInChildren<GuardFOVController>();
        guardRotatePatternOn = true;
        if(guardRotatePatternOn)
        {
            guardRotatePattern();
        }
    }

    void FixedUpdate()
    {
        if(delayTimeCounter == delayTime)
        {
            guardRotatePattern();
        }
        else
        {
            delayTimeCounter++;
        }
    }

    public void guardRotatePattern()
    {
        Debug.Log("guardRoate Start");
        delayTimeCounter = 0;
        if (!guardFOVController.isDrawingFOV)
        {
            Vector3 normalizedFOVDir = guardFOVController.FOVDirection.normalized;
            Vector3 normalVecOfDir = new Vector3(guardFOVController.FOVDirection.y, -guardFOVController.FOVDirection.x, 0); // 방향 벡터의 법선벡터(right)
            guardFOVController.FOVDirection = Vector3.Lerp(normalizedFOVDir, (normalizedFOVDir + normalVecOfDir).normalized, RotatePatternSpeed);
            guardFOVController.DrawGuardFOV();
        }
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