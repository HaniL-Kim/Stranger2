using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening_BrokerContoller : MonoBehaviour
{
    OpeningController openingController;
    Animator anim_Broker;
    Animator anim_Player;
    public bool playerKickedAway = false;

    private void Awake()
    {
        openingController = GameObject.Find("OpeningObjs").GetComponent<OpeningController>();
        anim_Broker = GetComponent<Animator>();
        anim_Player = GameObject.Find("OP_Player").GetComponent<Animator>();
    }

    private void Update()
    {
        if(playerKickedAway)
        {
            StartCoroutine("DelayCut_2");
        }
    }

    IEnumerator DelayCut_2()
    {
        playerKickedAway = false;
        yield return new WaitForSeconds(4f);
        openingController.StartCoroutine("StartCut2");
    }

    public void PlayMoveForwardAnim()
    {
        anim_Broker.SetTrigger("MoveForward");
        Debug.Log("Play MoveForward Anim");
    }

    public void PlayKickAnim()
    {
        anim_Broker.SetTrigger("Kick");
        Debug.Log("Play Kick Anim");
    }

    public void PlayPlayerKickedAwayAnim()
    {
        anim_Player.SetTrigger("KickedAway");
        Debug.Log("Play Player KickedAway Anim");
        playerKickedAway = true;
    }

}
