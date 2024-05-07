using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneScript : MonoBehaviour
{
    Animator animator;
    PlayerController playerRef;
    float yVelo;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        playerRef = GameManager.instance.GetPlayerController();


    }

    
    void Update()
    {
        yVelo = playerRef.GetComponent<Rigidbody2D>().velocity.y;
        if(yVelo != 0)
        {
            //animator.SetFloat("YVelocity", yVelo);
        }
    }


    void IdleCheck(bool flag)
    {
        animator.SetBool("IdleFlag", flag);
    }
}
