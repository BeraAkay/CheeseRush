using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAnimations : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    Transform[] bones;

    Vector3[] boneBases;

    Vector2 maxV;

    [SerializeField]
    Vector2 maxDistortion = Vector2.one * 0.1f;

    [Range(0,1), SerializeField]
    float verticalDistortionMult, horizontalDistortionMult;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!rb)
        {
            rb = GetComponentInParent<Rigidbody2D>();
        }

        boneBases = new Vector3[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            boneBases[i] = bones[i].localPosition;
        }

        PlayerController pCont = GetComponent<PlayerController>();
        if (pCont)
        {
            maxV = new Vector2(pCont.moveSpeed, pCont.jumpSpeed);
        }
        if(maxV.magnitude <= 0)
            maxV = Vector2.one * 10;

    }

    // Update is called once per frame
    void Update()
    {
        Distort();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.x != 0)
        {
            transform.GetChild(0).localScale = new Vector3(rb.velocity.x < 0 ? -1 : 1, 1, 1);

        }
    }

    void Distort()
    {
        float distX = (Mathf.Abs(rb.velocity.x) / maxV.x) * maxDistortion.x * horizontalDistortionMult;
        float distY = (rb.velocity.y / maxV.y) * maxDistortion.y * verticalDistortionMult;
        //add clamping for the squishing, not needed for stretching
        //if clamping will be used for both add a mult for dists instead

        Vector3 temp;

        temp = bones[0].localPosition;
        temp.x = boneBases[0].x - distX * 2;
        temp.y = boneBases[0].y - distY * 2;
        bones[0].localPosition = temp;

        temp = bones[1].localPosition;
        temp.x = boneBases[1].x - distX;
        temp.y = boneBases[1].y - distY;
        bones[1].localPosition = temp;
    }
}
