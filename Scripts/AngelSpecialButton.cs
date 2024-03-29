﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;

//This Script is attached to the ui element that acts as the angels skill button
public class AngelSpecialButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject player=null;
    float channelingTime=0f;






    void Start()
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (item.GetComponent<PhotonView>().IsMine) player = item;
        }

    }

    void Update()
    {
        
        channelingTime += Time.deltaTime;

        if(channelingTime >= 5f)
        {
            player.GetComponent<PC_LIIKKUU_10>().AngelSpecial(Mathf.Clamp(channelingTime, 0f, 5f));
        }
    }

    //Detect if a click occurs
    public void OnPointerDown(PointerEventData pointerEventData)
    {

        

        player.GetComponent<PC_LIIKKUU_10>().SpecialSkill();


        channelingTime = 0f;

    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {

        player.GetComponent<PC_LIIKKUU_10>().AngelSpecial(Mathf.Clamp(channelingTime, 0f, 5f));


    }
}
