using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10;
    public float jumpSpeed = 10;
    public float camFollowSpeed = 5;


    //LineRenderer lineRenderer;

    Rigidbody2D rb;

    Vector2 newV;
    float movementInput;

    [SerializeField]
    float deathY;

    [SerializeField]
    float minFallSpeed;

    public Camera playerCam;

    public bool kbm;

    [SerializeField]
    GameObject cloneFab;

    Transform cloneTransform;
    float cloneOffset;

    [SerializeField]
    KeyCode interruptCatKey;

    bool controlsEnabled = true;

    [SerializeField]
    GameObject dizzyEffects;


    //public TextMeshProUGUI gyroDataText;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        playerCam = Camera.main;

        deathY = GameManager.instance.screenSize.y + (GetComponent<BoxCollider2D>().size.y * transform.localScale.y)/2;//accouting for half the player height for above "waist"

        cloneOffset = LevelManager.instance.horizontalWidth * 1.15f;

        SetUpClone();

        controlsEnabled = true;
        dizzyEffects.SetActive(false);

        //Debug.Log(SystemInfo.supportsGyroscope);
        //Input.gyro.enabled = true;

        //lineRenderer = gameObject.AddComponent<LineRenderer>();

    }

    void Update()
    {
        movementInput = 0;
        movementInput = Input.acceleration.x;

        if(kbm)
            movementInput = Input.GetAxis("Horizontal");

        if (Input.GetKey(interruptCatKey))
        {
            GameManager.instance.InterruptCat();
        }
    }

    private void FixedUpdate()
    {
        if(playerCam.transform.position.y < transform.position.y)
            CameraFollow();


        else if(transform.position.y < playerCam.transform.position.y - deathY)
        {
            GameManager.instance.DeathTrigger();
        }
        if (Mathf.Abs(movementInput) > 0.1f && controlsEnabled)
        {
            newV = rb.velocity;
            //newV.x += movementInput * moveSpeed;
            if (newV.x > moveSpeed)
            {
                newV.x = Mathf.Min(movementInput * moveSpeed + newV.x, newV.x);
            }
            else
            {
                newV.x = movementInput * moveSpeed;
            }
            rb.velocity = newV;
        }
        
        if(rb.velocity.y < 0)
        {
            rb.gravityScale = 2;
        }
        else
        {
            rb.gravityScale = 1;
        }

        newV = rb.velocity;
        newV.y = Mathf.Max(rb.velocity.y, minFallSpeed);
        rb.velocity = newV;

        if (Mathf.Abs(transform.position.x) > cloneOffset * .25f)
        {
            UpdateClone();
        }
    }

    public void SetInterruptKey(KeyCode keyCode)
    {
        interruptCatKey = keyCode;
    }

    public void Jump(float mult)
    {
        rb.velocity = Vector2.up * jumpSpeed * mult;
    }

    void CameraFollow()
    {
        Vector3 target = playerCam.transform.position;
        target.y = transform.position.y;
        playerCam.transform.position = Vector3.Lerp(playerCam.transform.position, target, Time.fixedDeltaTime * camFollowSpeed);
    }


    void SetUpClone()//maybe go back to double clones if this is having ghost like effects
    {
        cloneTransform = Instantiate(cloneFab, transform).transform;
        cloneTransform.localPosition = new Vector2(-cloneOffset, 0);
    }

    void UpdateClone()
    {
        if (transform.position.x * cloneTransform.localPosition.x > 0)
        {
            cloneTransform.localPosition = -cloneTransform.localPosition;
        }
        if (Mathf.Abs(transform.position.x) > cloneOffset / 2)
        {
            transform.position = cloneTransform.position;
        }
    }

    public void ResetPlayer()
    {
        rb.velocity = Vector3.zero;
        transform.position = Vector3.zero;
    }


    public void DisableControls(float duration)
    {
        StartCoroutine(ControlDisabler(duration));
    }

    IEnumerator ControlDisabler(float duration)
    {
        controlsEnabled = false;
        dizzyEffects.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(duration);

        GetComponent<BoxCollider2D>().enabled = true;
        controlsEnabled = true;
        dizzyEffects.SetActive(false);
    }

}
