using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DefaultExecutionOrder(0)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    GameObject player;
    PlayerController playerController;

    [SerializeField]
    TextMeshProUGUI scoreText, highScoreGameOver, highScoreMainMenu, finalScoreText;

    [SerializeField]
    GameObject newHighScoreHeader, finalScoreHeader;

    [SerializeField]
    GameObject deathText;

    [SerializeField]
    float scoreMult;

    [SerializeField]
    GameObject playMenus, playUI, pauseMenu, mainMenu, gameOverDisplay;

    public Vector2 screenSize;

    //public event System.Action PlatformRecall;

    [SerializeField]
    CatScript cat;

    [SerializeField]
    TextMeshProUGUI textBox;

    public float Score
    {
        get
        {
            return score + additionalScore;
        }
    }

    float score;
    float additionalScore;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = false;

        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        highScoreGameOver.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        highScoreMainMenu.text = PlayerPrefs.GetInt("HighScore", 0).ToString();


        screenSize = Camera.main.ViewportToWorldPoint(Vector3.one);


        GoToMainMenu();
    }

    private void FixedUpdate()
    {
        score = Mathf.Max(Mathf.Floor(Camera.main.transform.position.y)* scoreMult, score);
        scoreText.text = Score.ToString();

    }

    public void InterruptCat()
    {
        if (cat.Interrupt())
        {
            cat.DisplayExpression(0);
        }
    }

    public void ScoreBoost(float scoreBoost)
    {
        additionalScore += scoreBoost;
    }

    
    public void DeathTrigger()
    {
        playMenus.SetActive(false);
        gameOverDisplay.SetActive(true);

        finalScoreText.text = score.ToString();

        if(score > PlayerPrefs.GetInt("HighScore", 0))//new highscore
        {
            highScoreGameOver.transform.parent.gameObject.SetActive(false);
            newHighScoreHeader.SetActive(true);
            finalScoreHeader.SetActive(false);
        }
        else
        {
            highScoreGameOver.transform.parent.gameObject.SetActive(true);

            newHighScoreHeader.SetActive(false);
            finalScoreHeader.SetActive(true);

        }

        player.SetActive(false);
        cat.enabled = false;
        SaveGame();//im aware this also checks for current vs high score its fine for now.
                   //ill probably just remove the check in the savegame func and just call it when a new high score is hit but later.
    }


    public void OnApplicationPause(bool pause)
    {
        
    }

    public void Pause()
    {
        Time.timeScale = 0;
        playUI.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Unpause()
    {
        pauseMenu.SetActive(false);
        playUI.SetActive(true);
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        LevelManager.instance.InitLevel();



        mainMenu.SetActive(false);
        playMenus.SetActive(true);
        playUI.SetActive(true);
        pauseMenu.SetActive(false);
        gameOverDisplay.SetActive(false);





        player.SetActive(true);
        cat.enabled = true;
        Time.timeScale = 1;
        
    }

    public void GoToMainMenu()
    {
        SaveGame();
        Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);
        player.transform.position = Vector3.zero;

        if(PlatformStackManager.instance != null)
            PlatformStackManager.instance.RecallPlatforms();

        cat.enabled = false;
        player.SetActive(false);

        GetComponent<BackgroundStep>().ResetBackground();

        score = 0;
        additionalScore = 0;
        Time.timeScale = 0;
        mainMenu.SetActive(true);
        if(PlayerPrefs.GetInt("HighScore", 0) > 0)
        {
            highScoreMainMenu.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            highScoreMainMenu.transform.parent.gameObject.SetActive(false);
        }


        playMenus.SetActive(false);


    }

    public void RestartGame()
    {
        PlatformStackManager.instance.RecallPlatforms();
        score = 0;
        additionalScore = 0;

        Camera.main.transform.position = new Vector3(0,0, Camera.main.transform.position.z);
        playerController.ResetPlayer();
        
        cat.enabled = false;
        cat.enabled = true;

        GetComponent<BackgroundStep>().ResetBackground();
        player.SetActive(true);

        
        LevelManager.instance.InitLevel();


        gameOverDisplay.SetActive(false);
        pauseMenu.SetActive(false);
        playUI.SetActive(true);
        playMenus.SetActive(true);
        player.SetActive(true);
        Time.timeScale = 1;
    }

    public void SaveGame()
    {
        if(PlayerPrefs.GetInt("HighScore", 0) < Score)
        {
            SetScore((int)score);
        }
    }

    public void ResetPlayerScore()
    {
        SetScore(0);
    }


    void SetScore(int _score)
    {

        PlayerPrefs.SetInt("HighScore", _score);
        highScoreGameOver.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        highScoreMainMenu.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }


    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }


}
