using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] TextMeshProUGUI scoreTMP;
    [SerializeField] TextMeshProUGUI pointTMP;

    const float addPointTime = 1.2f;
    const int numberOfNoDecreasingScore = 2;
    const int decreasingPoint = 2;

    public int score { get; private set; } = 0;
    Vector2 originLocalPosOfPointTMP;
    int leftNumberOfNoDecreasingScore;

    void SetUpScore()
    {
        score = 0;
        scoreTMP.text = score.ToString();
        pointTMP.alpha = 0f;
        originLocalPosOfPointTMP = pointTMP.transform.localPosition;
        leftNumberOfNoDecreasingScore = numberOfNoDecreasingScore + 1;
        DiceManager.RollDiceEvent += CheckNoCalculation;
    }

    public void AddScore(int point)
    {
        score += point;
        if (score < -99) score = -99;

        pointTMP.DOKill(false);
        pointTMP.transform.DOKill(false);
        pointTMP.transform.localPosition = originLocalPosOfPointTMP;
        if (point > 0)
        {
            pointTMP.color = Color.green;
            pointTMP.text = "+" + point.ToString();
        }
        else if (point < 0)
        {
            pointTMP.color = Color.red;
            pointTMP.text = point.ToString();
        }

        pointTMP.alpha = 1f;
        pointTMP.DOFade(0f, addPointTime);
        pointTMP.transform.DOLocalMoveY(originLocalPosOfPointTMP.y + 100, addPointTime);

        scoreTMP.text = score.ToString();
    }

    void CheckNoCalculation()
    {
        if (CalculateManager.instance.NumberOfCorrectCalculation <= 0)
        {
            if (leftNumberOfNoDecreasingScore > 0)
                leftNumberOfNoDecreasingScore -= 1;
            else
            {
                AddScore(-decreasingPoint);
            }
        }

        CalculateManager.instance.NumberOfCorrectCalculation = 0;
    }

    private void Start()
    {
        SetUpScore();
    }

    private void OnDestroy()
    {
        DiceManager.RollDiceEvent -= CheckNoCalculation;
    }
}