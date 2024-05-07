using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    bool breakable;

    bool moving;

    float moveRange, moveSpeed, anchor;
    int moveDir = 1;

    Stack<GameObject> mom;

    GameObject pickup;

    Animator breakAnimator;

    private void FixedUpdate()
    {
        if (moving)
        {
            Move();
        }

        if (Camera.main.transform.position.y - GameManager.instance.screenSize.y - 0.2f > transform.position.y && gameObject.name != "Ground")
        {
            BackToStack();
        }

    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && collision.relativeVelocity.y == 0)
        {
            collision.gameObject.GetComponent<PlayerController>().Jump(1);
            if(breakable)
                BreakPlatform();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && collision.relativeVelocity.y < 0)
        {
            collision.gameObject.GetComponent<PlayerController>().Jump(1);
            if (breakable)
                BreakPlatform();
        }
    }

    void BreakPlatform()
    {
        //breakAnim
        StartCoroutine(BreakRoutine());
    }

    IEnumerator BreakRoutine()
    {
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        breakAnimator.Rebind();
        breakAnimator.Update(0f);

        breakAnimator.SetTrigger("Break");
        
        yield return new WaitUntil(() => breakAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlatformBreak"));
        yield return new WaitUntil(() => breakAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= .95f);

        GetComponent<Collider2D>().enabled = true;
        

        BackToStack();
    }

    private void OnDisable()
    {
        moving = false;
        moveRange = 0;
        moveSpeed = 0;
        anchor = 0;
        if (pickup)
        {
            Destroy(pickup);
        }
        if (breakable)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }

    }


    public void SetBreakable(bool _breakable)
    {
        breakable = _breakable;
        if (breakable)
        {
            breakAnimator = transform.GetChild(1).GetComponent<Animator>();
        }
    }

    public void MovePlatform( float _moveRange, float _moveSpeed, float _anchor)
    {
        moving = true;

        moveRange = _moveRange;
        moveSpeed = _moveSpeed;

        anchor = _anchor;

        moveDir = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
    }

    public void SetPickup(GameObject pickupObject)
    {
        pickup = Instantiate(pickupObject,transform);
        float yOffset = pickup.GetComponent<BoxCollider2D>().size.y * 0.75f * pickup.transform.localScale.y;
        pickup.transform.localPosition = new Vector3(0, yOffset, 0);
        pickup.SetActive(true);
    }

    private void Move()
    {
        if (moveDir * (anchor + (moveRange * moveDir) - transform.position.x) > 0)
        {
            Vector3 newPos = transform.position;
            newPos.x += Time.fixedDeltaTime * moveSpeed * moveDir;
            transform.position = newPos;
        }
        else
        {
            moveDir *= -1;
        }
    }

    public void BackToStack()
    {
        mom.Push(gameObject);
        PlatformStackManager.instance.platformRecall -= BackToStack;
        gameObject.SetActive(false);
    }

    public void SetMom(Stack<GameObject> _mom)
    {
        mom = _mom;
    }

}
