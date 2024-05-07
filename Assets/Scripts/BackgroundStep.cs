using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundStep : MonoBehaviour
{
    [SerializeField]
    Transform V0, V1, V2;

    Transform prev, current, next, temp;

    float yOffset;

    Transform playerTransform;
    void Start()
    {
        playerTransform = GameManager.instance.GetPlayerTransform();

        yOffset = V2.position.y - V1.position.y;

        prev = V0;
        current = V1;
        next = V2;
    }

    void FixedUpdate()
    {
        if(playerTransform.position.y - current.position.y >= yOffset/2)
        {

            temp = current;
            current = next;
            next = prev;
            prev = temp;

            prev.GetComponent<SpriteRenderer>().sortingOrder = 0;
            current.GetComponent<SpriteRenderer>().sortingOrder = 1;
            next.GetComponent<SpriteRenderer>().sortingOrder = 2;

            next.Translate(new Vector3(0, yOffset * 3, 0));
        }
    }

    public void ResetBackground()
    {
        if(yOffset == 0)
            yOffset = V2.position.y - V1.position.y;


        prev = V0;
        current = V1;
        next = V2;

        prev.GetComponent<SpriteRenderer>().sortingOrder = 0;
        current.GetComponent<SpriteRenderer>().sortingOrder = 1;
        next.GetComponent<SpriteRenderer>().sortingOrder = 2;

        prev.transform.position = new Vector3(0, -yOffset);
        current.transform.position = Vector3.zero;
        next.transform.position = new Vector3(0, yOffset);
    }
}
