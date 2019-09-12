using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepSound : MonoBehaviour
{
    public Animator animFootStep;

    private void Awake()
    {
        animFootStep = GetComponent<Animator>();
    }

    public void PlayFootStepSoundEnd()
    {
        animFootStep.Play("FootStepSoundEnd");
    }


}
