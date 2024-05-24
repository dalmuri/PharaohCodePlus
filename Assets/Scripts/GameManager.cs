using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] GameObject gameOverWindowGO;
    [SerializeField] GameObject progressBtnGO;
    [SerializeField] TMP_Text gameOverScoreTMP;
    [SerializeField] TMP_Text timerTMP;

    const string TIMER_SETTING_KEY = "TIMER_SETTING_KEY";

    public static Action TimeOver;
    readonly WaitForSecondsRealtime oneSecond = new WaitForSecondsRealtime(1f);
    readonly Color colorSandGlass= new Color(0.9098039f, 0.682353f, 0.4156863f);

    public int timeLimit { get; private set; } = -1;
    public int numberOfGettingHandInTurn { get; private set; } = 10;

    int leftTime;
    Coroutine runningTimerCoroutine;

    void SetUpSettings()
    {
        switch (PlayerPrefs.GetInt(TIMER_SETTING_KEY))
        {
            case 0:
                timeLimit = -1;
                break;
            case 1:
                timeLimit = 30;
                break;
            case 2:
                timeLimit = 60;
                break;
            case 3:
                timeLimit = 90;
                break;
            case 4:
                timeLimit = 120;
                break;
        }
    }

    public void RollDice()
    {
        StartCoroutine(RollDiceCo());
    }

    IEnumerator RollDiceCo()
    {
        leftTime = 0;
        if(runningTimerCoroutine != null) StopCoroutine(runningTimerCoroutine);
        progressBtnGO.SetActive(false);
        yield return StartCoroutine(NumberSquareManager.instance.FillPyramidCo());

        yield return StartCoroutine(DiceManager.instance.RollDiceCo());
        NumberSquareManager.instance.SetNumberSquareDraggable(true);

        runningTimerCoroutine = StartCoroutine(TimerCo(timeLimit));
        progressBtnGO.GetComponent<ProgressButton>().CheckProgressBtnState();
        progressBtnGO.SetActive(true);
    }

    IEnumerator TimerCo(int timeLimit)
    {
        timerTMP.color = colorSandGlass;

        if(timeLimit == -1) // infinite time
        {
            timerTMP.text = "¡Ä";
            timerTMP.fontSize = 160;
        }
        else
        {
            leftTime = timeLimit;
            timerTMP.text = leftTime.ToString();
            timerTMP.fontSize = 80;

            while (leftTime > 0)
            {
                yield return oneSecond;
                leftTime -= 1;
                timerTMP.text = leftTime.ToString();
            }

            timerTMP.color = Color.red;
            NumberSquareManager.instance.SetNumberSquareDraggable(false);
        }
    }

    public void EndGame()
    {
        DragManager.instance.canAllDrag = false;
        progressBtnGO.SetActive(false);
        gameOverWindowGO.SetActive(true);
        gameOverScoreTMP.text = ScoreManager.instance.score.ToString();
    }

    public void RestartGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void StartLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    private void Start()
    {
        SetUpSettings();
        gameOverWindowGO.SetActive(false);
        progressBtnGO.SetActive(true);
    }
}
