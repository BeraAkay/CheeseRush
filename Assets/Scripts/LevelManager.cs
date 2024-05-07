using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[DefaultExecutionOrder(2)]
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    PlayerController player;

    [SerializeField]
    float spawnRangeRatio;

    [SerializeField]
    float drawDistance;

    [HideInInspector]
    public float horizontalWidth;
    float spawnWidth;


    float platformWidth;
    [SerializeField]
    float baseChanceToSpawnMovable;
    float currentChanceToSpawnMovable;
    [SerializeField]
    float movableChanceGrowth;

    [SerializeField]
    float platformBaseMoveRange, moveSpeed;//, platformBaseMoveSpeed;
    //[SerializeField]
    //float platformMoveSpeedGrowth, platformMoveRangeGrowth;

    [SerializeField]
    AnimationCurve movingPlatformSpeedMults, breakableSpawns;//,movingPlatformRanges;

    PlatformStackManager stackManager;

    Vector2 maxJumpDistance;

    Platform lastSpawnedPlatform, newPlatform;
    [SerializeField]
    float baseChanceToAddBreakable;
    float currentChanceToAddBreakable;
    [SerializeField]
    float breakableChanceGrowth;

    [SerializeField]
    float scoreRangeForMovingMin, scoreRangeForMovingMax, scoreRangeForBreakableMin, scoreRangeForBreakableMax;

    [SerializeField]
    GameObject[] pickupTypes;

    [SerializeField, Range(0, 1)]
    float pickupSpawnChance = 0.1f;

    float currentPickupSpawnChance;
    [SerializeField, Range(0, 1)]
    float pickupSpawnChanceGrowth = 0.01f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        stackManager = GetComponent<PlatformStackManager>();
        player = GameManager.instance.GetPlayerController();

        horizontalWidth = GameManager.instance.screenSize.x * 2;

        platformWidth = stackManager.GetPlatformSize();


        spawnWidth = horizontalWidth - platformWidth;

        maxJumpDistance = new Vector2(player.moveSpeed, player.jumpSpeed/2) * 0.75f;//jump height peak takes a second to achieve so max dists are as shown,
                                                                                    //maybe i will just make this manually adjusted as a public variable
                                                                                    //recalcing this as the game goes on and increasing player speeds based on progress
                                                                                    //might be the way to go

        currentChanceToAddBreakable = baseChanceToAddBreakable;
        currentPickupSpawnChance = pickupSpawnChance;

    }

    // Update is called once per frame
    void Update()
    {
        GenerateLevel();

    }

    private void FixedUpdate()
    {
        
    }




    public void InitLevel()
    {
        currentChanceToAddBreakable = baseChanceToAddBreakable;
        currentPickupSpawnChance = pickupSpawnChance;
        lastSpawnedPlatform = GivePlatform(false);
        lastSpawnedPlatform.transform.position = new Vector2(0, -5);
        lastSpawnedPlatform.gameObject.SetActive(true);

    }

    void GenerateLevel()
    {
        if (lastSpawnedPlatform)
        {
            if (lastSpawnedPlatform.transform.position.y - player.transform.position.y < drawDistance)
                GeneratePlatform();
        }
    }


    void GeneratePlatform()
    {
        bool breakable = currentChanceToAddBreakable > Random.Range(0.0f, 1.0f);
        if (breakable)
        {
            currentChanceToAddBreakable = breakableSpawns.Evaluate(Mathf.Clamp01((GameManager.instance.Score - scoreRangeForBreakableMin) / (scoreRangeForBreakableMax - scoreRangeForBreakableMin)));//baseChanceToAddBreakable;//change this to the diff curve evaluation
        }
        else
        {
            currentChanceToAddBreakable += breakableChanceGrowth;
        }
        newPlatform = GivePlatform(breakable);

        float npX = Random.Range(-spawnWidth / 2, spawnWidth / 2);

        float npY = lastSpawnedPlatform.transform.position.y + Random.Range(maxJumpDistance.y * (1 - spawnRangeRatio), maxJumpDistance.y);

        float mSpeed = moveSpeed * movingPlatformSpeedMults.Evaluate(Mathf.Clamp01((GameManager.instance.Score - scoreRangeForMovingMin)/(scoreRangeForMovingMax-scoreRangeForMovingMin)));

        if(mSpeed > 0.25f)
        {
            newPlatform.MovePlatform(platformBaseMoveRange, mSpeed, npX);
        }


        if(currentPickupSpawnChance > Random.Range(0.0f, 1.0f))
        {
            newPlatform.SetPickup(pickupTypes[Random.Range(0,pickupTypes.Length)]);
            currentPickupSpawnChance = pickupSpawnChance;
        }
        else
        {
            currentPickupSpawnChance += pickupSpawnChanceGrowth;
        }


        newPlatform.transform.position = new Vector2(npX, npY);
        newPlatform.gameObject.SetActive(true);

        lastSpawnedPlatform = newPlatform;
    }

    /*
    void OLDGeneratePlatform()
    {

        newPlatform = GivePlatform(false);
        //make a vector to use instead of max jump dist, and set it via difficulty * maxjumpdist so if diff is 1, max jump dist is active otherwise its a percentage
        //maybe increase the player speed too as level goes on so max jump dist also increases etc etc.
        float pad = 0.1f;

        float toAdd = Random.Range(platformWidth / 2 + pad, (platformWidth / 2) + maxJumpDistance.x + (platformWidth / 2));
        float npX = (lastSpawnedPlatform.transform.position.x + (Random.Range(0, 2) == 1 ? -1 : 1) * toAdd);//add % magic to limit it to screen area

        npX = ((npX + spawnWidth / 2) % spawnWidth) - spawnWidth/2;


        float npY = lastSpawnedPlatform.transform.position.y + Random.Range(maxJumpDistance.y * (1 - spawnRangeRatio), maxJumpDistance.y);


        npX = Mathf.Clamp(npX, -spawnWidth/2, spawnWidth/2);
        
        //add clamp
        //actually use % to limit it to the screen space using horizontalWidth;

        //Debug.Log(npX.ToString() + ", " + npY.ToString());

        float mRange = 0;

        if (currentChanceToSpawnMovable > Random.Range(0.0f, 1.0f))
        {
            newPlatform.InitPlatform(false, platformBaseMoveRange, platformBaseMoveSpeed, npX);


            mRange = platformBaseMoveRange;

            currentChanceToSpawnMovable = baseChanceToSpawnMovable; 
        }
        else
        {
            currentChanceToSpawnMovable += movableChanceGrowth;

            newPlatform.InitPlatform(false);
        }



        newPlatform.transform.position = new Vector2(npX, npY);
        newPlatform.gameObject.SetActive(true);

        //newPlatform.GetComponent<Platform>().breakable = 
        //newPlatform.GetComponent<Platform>().moving = //depending on score, after a while it should be all moving so like at the start, no movers, then some movers, then all movers

        lastSpawnedPlatform = newPlatform;



        if (currentChanceToAddBreakable > Random.Range(0.0f, 1.0f))
        {
            AddBreakablePlatformToLayer(newPlatform.transform.position, platformWidth, mRange);

            currentChanceToAddBreakable = baseChanceToAddBreakable;//base will prob be zero but ill change this after testing
            //currentChanceToAddBreakable = 0;
        }
        else
        {
            currentChanceToAddBreakable += breakableChanceGrowth;
        }

    }
    */
    /*
    void AddBreakablePlatformToLayer(Vector3 platformCoords, float platformWidth, float platformMoveRange)
    {
        Platform breakable = GivePlatform(true);

        breakable.InitPlatform(true);


        float breakableWidth = breakable.GetComponent<BoxCollider2D>().size.x;

        float platSideX, nonPlatSideX;

        int platformDir = platformCoords.x > 0 ? 1 : -1;

        platSideX = platformCoords.x - platformDir * ((platformWidth / 2) + (breakableWidth / 2) + platformMoveRange + 0.2f);
        nonPlatSideX = (spawnWidth / 2) * -platformDir;



        float lX, rX;
        lX = Mathf.Min(platSideX, nonPlatSideX);
        rX = Mathf.Max(platSideX, nonPlatSideX);

        breakable.transform.position = new Vector2( Random.Range(lX,rX), platformCoords.y);
        breakable.gameObject.SetActive(true);
    }
    */
    Platform GivePlatform(bool breakable)
    {
        return stackManager.GetPlatform(breakable).GetComponent<Platform>();
    }

}
