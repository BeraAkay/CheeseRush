using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatScript : MonoBehaviour
{
    [SerializeField]
    RectTransform catExpressions;

    Image expressionSprite;

    int expressionStage;//0 ready for expression, 1 expression appear, -1 expression disappear

    Color expColorRef;

    [SerializeField]
    float expressionAppearRate;
    [SerializeField]
    float expressionDisappearRate;

    [SerializeField]
    float followSpeed = 100;

    [SerializeField]
    Transform target;

    [SerializeField]
    SpriteRenderer attackSprite;

    Rigidbody2D playerRB;

    [SerializeField]
    SpriteRenderer shadeSpriteRenderer;

    Material shadeMat;

    [SerializeField]
    float sequenceCooldown;

    float sequenceTimer;

    float appear, attackProgress;

    float radius;

    [SerializeField]
    float appearRate, shrinkRate, revertRate, recoverRate;

    [SerializeField]
    float maxRadius, minRadius;

    int stateID = 0;//0 neutral, 1 appear, -1 revert, 2 shrink, 3 attack, -3 recover

    [SerializeField]
    float attackPower;

    Color spColor;

    [SerializeField]
    RectTransform tailSprite;

    Vector3 tailInPosition;
    Vector3 tailOutPosition;

    [HideInInspector]
    public int interruptKey;

    [SerializeField]
    Transform[] tapes;//0: left, 1: right

    [SerializeField]
    float tapeRange;
    Vector3 tapeInPosition, tapeOutPosition;
    

    void Start()
    {
        playerRB = target.GetComponent<Rigidbody2D>();

        shadeMat = shadeSpriteRenderer.material;

        tailOutPosition = tailSprite.position;
        tailInPosition = new Vector3(tailOutPosition.x, -tailOutPosition.y, tailOutPosition.z);

        tapeOutPosition = tapes[0].localPosition;
        tapeInPosition = tapeOutPosition + new Vector3(tapeRange, 0, 0);
    }

    private void OnEnable()
    {
        InitValues();
    }

    private void OnDisable()
    {
        InitValues();
    }

    void InitValues()
    {
        if (!shadeMat)
        {
            shadeMat = shadeSpriteRenderer.material;
        }

        stateID = 0;
        appear = 0;
        radius = maxRadius;
        shadeMat.SetFloat("_MainAlpha", appear);
        shadeMat.SetFloat("_Radius", radius);

        attackProgress = 0;
        if (attackSprite != null)
        {
            spColor = attackSprite.color;
            spColor.a = attackProgress;
            attackSprite.color = spColor;
        }
        

        expressionStage = 0;
        
        if (expressionSprite != null)
        {
            expColorRef = expressionSprite.color;
            expColorRef.a = 0;
            expressionSprite.color = expColorRef;
        }

        if(tailSprite != null && tailOutPosition != Vector3.zero)
            tailSprite.position = tailOutPosition;

        if(tapeOutPosition != Vector3.zero)
        {
            tapes[0].localPosition = tapeOutPosition;
            tapes[1].localPosition = -tapes[0].localPosition;
        }
        tapes[0].GetComponent<BoxCollider2D>().enabled = false;
        tapes[1].GetComponent<BoxCollider2D>().enabled = false;
    }

    void FixedUpdate()
    {
        Follow();
        SpotlightCycle();

        
        CatExpression(expressionSprite);
        
    }
    void Follow()
    {
        transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position, Time.fixedDeltaTime * followSpeed);
        Vector3 spotCoords = Vector3.zero;
        if (stateID != 1 && stateID != 0)
        {
            spotCoords = Vector3.Lerp(spotCoords,
                (target.position - Camera.main.transform.position) / shadeSpriteRenderer.transform.localScale.x,
                Time.fixedDeltaTime * (maxRadius-radius)/(maxRadius-minRadius) * followSpeed);
            spotCoords.z = 0;
        }
        shadeMat.SetVector("_SpotCoords", spotCoords);

    }

    void SpotlightCycle()
    {
        float t;
        switch (stateID)
        {
            case 0:
                sequenceTimer += Time.fixedDeltaTime;
                if (sequenceTimer >= sequenceCooldown)
                {
                    sequenceTimer = 0;
                    stateID = 1;
                }
                break;
            case 1:
                appear = Mathf.Lerp(0, 1, appear + (appearRate * Time.fixedDeltaTime));//XD
                tapes[0].localPosition = Vector3.Lerp(tapeOutPosition, tapeInPosition, appear);
                tapes[1].localPosition = -1 * tapes[0].localPosition;
                shadeMat.SetFloat("_MainAlpha", appear);
                if (appear == 1)
                {
                    stateID = 2;
                    radius = maxRadius;
                    interruptKey = Random.Range((int)'a',(int)'z'+1);
                    tailSprite.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = ((char)(interruptKey-32)).ToString();//-32 to make it upper case instead of calling toUpper
                    GameManager.instance.GetPlayerController().SetInterruptKey((KeyCode)interruptKey);
                }
                else if(appear > 0.5f)
                {
                    tapes[0].GetComponent<BoxCollider2D>().enabled = true;
                    tapes[1].GetComponent<BoxCollider2D>().enabled = true;
                }
                break;
            case 2:
                t = ((radius - minRadius) / (maxRadius - minRadius)) - (shrinkRate * Time.fixedDeltaTime);
                radius = Mathf.Lerp(minRadius, maxRadius, t);
                shadeMat.SetFloat("_Radius", radius);

                tailSprite.position = Vector3.Lerp(tailOutPosition, tailInPosition, (1-t)*2);
                if (radius == minRadius)
                {
                    stateID = 3;
                    tailSprite.position = tailOutPosition;
                }
                break;
            case 3:

                int atkDir = Random.Range(0, 2) > 0 ? 1 : -1;
                attackSprite.transform.parent.localScale = new Vector3(atkDir, 1, 1);

                attackProgress = 1;

                spColor = attackSprite.color;

                spColor.a = attackProgress;

                attackSprite.color = spColor;

                //tapes[0].localPosition = tapeOutPosition;
                //tapes[1].localPosition = -tapeOutPosition;

                tapes[0].GetComponent<BoxCollider2D>().enabled = false;
                tapes[1].GetComponent<BoxCollider2D>().enabled = false;

                GameManager.instance.GetPlayerController().DisableControls(1.5f);

                attackSprite.transform.position = target.transform.position;
                //attackSprite.flipY = dir;
                Vector2 v = playerRB.velocity;
                v.x = attackPower * atkDir;
                v.y -= attackPower * 0.35f;
                playerRB.velocity = v;

                DisplayExpression(1);

                stateID = -3;

                break;
            case -1:
                t = appear - revertRate * Time.fixedDeltaTime;
                appear = Mathf.Lerp(0, 1, t);
                shadeMat.SetFloat("_MainAlpha", appear);
                radius = Mathf.Lerp(minRadius, maxRadius, 1-t);
                shadeMat.SetFloat("_Radius", radius);

                tailSprite.position = Vector3.Lerp(tailOutPosition, tailSprite.position, t);
                if(Vector3.Distance(tapeInPosition, tapes[0].localPosition) < tapeRange)
                {
                    spColor.a = appear;
                    tapes[0].GetComponent<SpriteRenderer>().color = spColor;
                    tapes[1].GetComponent<SpriteRenderer>().color = spColor;
                    tapes[0].localPosition = Vector3.Lerp(tapeOutPosition, tapeInPosition, appear);
                    tapes[1].localPosition = -1 * tapes[0].localPosition;
                }
                if (appear == 0 && radius == maxRadius)
                {
                    stateID = 0;
                    if(tapes[0].GetComponent<SpriteRenderer>().color.a < 1)
                    {
                        spColor.a = 1;
                        tapes[0].GetComponent<SpriteRenderer>().color = spColor;
                        tapes[1].GetComponent<SpriteRenderer>().color = spColor;
                        spColor.a = 0;
                    }
                }
                break;
            case -3:
                attackProgress = Mathf.Lerp(0, 1, attackProgress - (recoverRate * Time.fixedDeltaTime));

                spColor = attackSprite.color;

                spColor.a = attackProgress;

                attackSprite.color = spColor;

                tapes[0].GetComponent<SpriteRenderer>().color = spColor;
                tapes[1].GetComponent<SpriteRenderer>().color = spColor;

                if (attackProgress == 0)
                {
                    spColor.a = 1;
                    tapes[0].GetComponent<SpriteRenderer>().color = spColor;
                    tapes[1].GetComponent<SpriteRenderer>().color = spColor;
                    spColor.a = 0;
                    tapes[0].localPosition = tapeOutPosition;
                    tapes[1].localPosition = -tapeOutPosition;
                    stateID = -1;
                }
                break;
            default:
                break;
        }
    }

    void CatExpression(Image sprite)
    {
        switch (expressionStage)
        {
            case 0:
                break;
            case 1:
                expColorRef = expressionSprite.color;
                expColorRef.a = Mathf.Lerp(0, 1, expColorRef.a + expressionAppearRate * Time.fixedDeltaTime);
                expressionSprite.color = expColorRef;
                if (expColorRef.a == 1)
                {
                    expressionStage = -1;
                }
                break;
            case -1:
                expColorRef = expressionSprite.color;
                expColorRef.a = Mathf.Lerp(0, 1, expColorRef.a - expressionDisappearRate * Time.fixedDeltaTime);
                expressionSprite.color = expColorRef;
                if(expColorRef.a == 0)
                {
                    expressionStage = 0;
                }
                break;
            default:
                break;
        }
        

    }

    public bool DisplayExpression(int type)
    {
        if(expressionStage == 0)
        {
            expressionSprite = catExpressions.GetChild(type).GetComponent<Image>();
            
            expressionStage = 1;
        }
        return false;

    }


    public bool Interrupt()
    {
        if (stateID == 2)//stateID > 0 && stateID != 3)
        {
            tapes[0].GetComponent<BoxCollider2D>().enabled = false;
            tapes[1].GetComponent<BoxCollider2D>().enabled = false;
            stateID = -1;
            return true;
        }
        return false;
    }

}
