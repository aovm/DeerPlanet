using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class LevelManager : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField]
    [Tooltip("Level time limit in seconds")]
    float levelTime = 30;
    [SerializeField]
    [Tooltip("Level time limit in seconds for third start")]
    float levelTimeStar = 40;
    [SerializeField]
    [Tooltip("Level Number")]
    int levelNumber = 1;

    [Header("Ui Elements")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text bioplasticsScoreText;
    [SerializeField] GameObject levelCompletedUi;
    [SerializeField] GameObject outOfTimeUi;
    [SerializeField] GameObject deathUi;
    [SerializeField] GameObject playerHpUi;
    [SerializeField] GameObject introUi;



    public static LevelManager instance;
    private int score = 0;
    private int bioplasticsScore = 0;
    private bool levelEnded = false;

    [HideInInspector]
    public int myHiddenVariable;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //set ui level name
        introUi.transform.Find("Title").GetComponent<TMP_Text>().text = "Welcome to " + SceneManager.GetActiveScene().name;


        //move uis into canvas 
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas != null)
        {
            List<RectTransform> childrenList = new List<RectTransform>();
            foreach (RectTransform tr in transform)
                childrenList.Add(tr);
            foreach (RectTransform tr in childrenList)
                tr.SetParent(canvas.transform);
        }




    }

    void Update()
    {
        UpdateTime();
        UpdateScore();
        playerHpUi.GetComponent<Slider>().value = (float)PlayerHealth.instance.playerHP / PlayerHealth.instance.maxHP;
    }

    void UpdateScore()
    {

        bioplasticsScoreText.text = "Bio: " + bioplasticsScore.ToString();
        scoreText.text = "Score: " + score.ToString();
        if (GameObject.FindGameObjectsWithTag("Collectable").Length == 0)
            EndLevel(1);

    }

    void HideAllUi()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            foreach (Transform child in canvas.transform)
                child.gameObject.SetActive(false);
        }
    }


    void UpdateTime()
    {
        levelTime = Mathf.Clamp(levelTime - Time.deltaTime, 0, 1000);
        int minutes = Mathf.FloorToInt(levelTime / 60);
        int seconds = Mathf.FloorToInt(levelTime % 60);
        timeText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");

        if (levelTime == 0)
            EndLevel(2);
    }

    public void CloseIntro()
    {
        introUi.SetActive(false);
    }


    public void EndLevel(int type)
    {
        //type: 1 = win, 2 = out of time, 3 = death
        if (!levelEnded)
        {
            levelEnded = true;
            GameObject.FindGameObjectWithTag("Player").SetActive(false);
            GameObject uiToShow = type == 1 ? levelCompletedUi : (type == 2 ? outOfTimeUi : deathUi);
            HideAllUi();
            uiToShow.SetActive(true);
            if(type == 1)   //Condiciones para el numero de estrellas conseguidar por nivel
            {
                if(PlayerPrefs.GetInt("Lv" + levelNumber) == 1)
                {
                    if (PlayerHealth.instance.playerHP == PlayerHealth.instance.maxHP)
                    {
                        PlayerPrefs.SetInt("Lv" + levelNumber, 2);
                        if (levelTime > levelTimeStar)
                            PlayerPrefs.SetInt("Lv" + levelNumber, 3);
                    }   
                }
                else if(PlayerPrefs.GetInt("Lv" + levelNumber) == 2)
                {
                    if(levelTime > levelTimeStar)
                        PlayerPrefs.SetInt("Lv" + levelNumber, 3);
                }
                else if(PlayerPrefs.GetInt("Lv" + levelNumber) == 0)
                {
                    PlayerPrefs.SetInt("Lv" + levelNumber, 1);
                    if (PlayerHealth.instance.playerHP == PlayerHealth.instance.maxHP)
                    {
                        PlayerPrefs.SetInt("Lv" + levelNumber, 2);
                        if (levelTime > levelTimeStar)
                            PlayerPrefs.SetInt("Lv" + levelNumber, 3);
                    }
                }
            }
        }
    }

    public void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
        else
            SceneManager.LoadScene(0);
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void IncreaseScore(int score_ = 0, int bio = 0)
    {
        score += score_;
        bioplasticsScore += bio;
    }








}
