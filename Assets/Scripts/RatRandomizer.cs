using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatRandomizer : MonoBehaviour
{
    public SpriteRenderer iris, fur;

    public Transform dynamicsParent;

    public float waitTime = 3;
    float waiter;
    void Start()
    {
        RandomizeRat();
    }

    /*
    private void FixedUpdate()
    {
        waiter += Time.fixedDeltaTime;
        if(waiter > waitTime)
        {
            RandomizeRat();
            waiter = 0;
        }
    }
    */

    void RandomizeRat()
    {
        Color color = Random.ColorHSV(0, 1, 0.3f, 1, 0.3f, 0.7f);
        iris.color = color;

        color = Random.ColorHSV(0, 1, 0.3f, 1, 0.3f, 0.7f);
        fur.color = color;
    }

    void RandomizeRatDynamics()
    {
        Color color;
        foreach (SpriteRenderer spRend in dynamicsParent.GetComponentsInChildren<SpriteRenderer>())
        {
            color = Random.ColorHSV(0, 1, 0.3f, 1, 0.3f, 0.7f);
            spRend.color = color;
        }
    }
}
