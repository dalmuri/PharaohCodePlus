using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProgressButton : MonoBehaviour
{
    readonly Color colorCyan = new Color(0f, 1f, 0.8663819f);
    readonly Color colorRed = new Color(1f, 0.1556604f, 0.1556604f);
    readonly Color colorYelloGreen = new Color(0.01829386f, 1f, 0f);
    enum ProgressBtnState { Start, RollDice, GiveUp, End }
    ProgressBtnState progressBtnState = ProgressBtnState.Start;
    bool isGameStarted = false;

    [SerializeField] TMP_Text progressBtnTMP;

    void SetUpProgressButton()
    {
        isGameStarted = false;
        CheckProgressBtnState();
    }

    public void OnClick()
    {
        switch (progressBtnState)
        {
            case ProgressBtnState.Start:
                GameManager.instance.RollDice();
                isGameStarted = true;
                break;
            case ProgressBtnState.RollDice:
                GameManager.instance.RollDice();
                break;
            case ProgressBtnState.GiveUp:
                HandManager.instance.GiveUp();
                break;
            case ProgressBtnState.End:
                GameManager.instance.EndGame();
                break;
        }
    }

    public void CheckProgressBtnState()
    {
        if (!isGameStarted)
        {
            progressBtnState = ProgressBtnState.Start;
            progressBtnTMP.text = "Start";
            progressBtnTMP.color = colorCyan;
        }
        else // 게임이 진행 중인 경우
        {
            if (HandManager.instance.GetNumberOfHand() > 0) // Give Up 버튼 활성화
            {
                progressBtnState = ProgressBtnState.GiveUp;
                progressBtnTMP.text = "Give Up";
                progressBtnTMP.color = colorRed;
            }
            else if (!NumberSquareManager.instance.isPyramidFull) // 게임 종료 버튼 활성화
            {
                progressBtnState = ProgressBtnState.End;
                progressBtnTMP.fontSize = 62;
                progressBtnTMP.text = "게임 종료";
                progressBtnTMP.color = colorYelloGreen;
            }
            else // Roll Dice 버튼 활성화
            {
                progressBtnState = ProgressBtnState.RollDice;
                progressBtnTMP.text = "Roll Dice";
                progressBtnTMP.color = colorCyan;
            }
        }
    }

    private void Start()
    {
        SetUpProgressButton();
    }
}
