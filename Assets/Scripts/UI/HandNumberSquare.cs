using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandNumberSquare : MonoBehaviour
{
    [SerializeField] Image numberSquareImage;
    [SerializeField] TMP_Text numberTMP;

    [SerializeField] Sprite spriteYellow;
    [SerializeField] Sprite spriteBlue;
    [SerializeField] Sprite spriteRed;
    [SerializeField] Sprite spritePurple;

    [SerializeField] Sprite spriteEmpty;

    public NumberInfo numberInfo { get; private set; }
    public SquareColor squareColor { get; private set; }
    public int number { get; private set; }
    public int point { get; private set; }

    public void SetUp(NumberInfo numberInfo)
    {
        this.numberInfo = numberInfo;

        this.squareColor = numberInfo.squareColor;
        this.number = numberInfo.number;
        this.point = numberInfo.point;

        numberTMP.text = this.number == 0 ? "" : this.number.ToString();
        switch (squareColor)
        {
            case SquareColor.Yellow:
                numberSquareImage.sprite = spriteYellow;
                break;
            case SquareColor.Blue:
                numberSquareImage.sprite = spriteBlue;
                break;
            case SquareColor.Red:
                numberSquareImage.sprite = spriteRed;
                break;
            case SquareColor.Purple:
                numberSquareImage.sprite = spritePurple;
                break;
            case SquareColor.Empty:
                numberSquareImage.sprite = spriteEmpty;
                break;
        }
    }

    public void OnClick()
    {
        HandManager.instance.SelectHandNumberSquare(this);
    }
}