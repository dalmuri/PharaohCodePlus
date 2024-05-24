using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckDisplaySquare : MonoBehaviour
{
    [SerializeField] SpriteRenderer deckDisplaySquare;
    [SerializeField] TMP_Text numberTMP;

    [SerializeField] Sprite spriteYellow;
    [SerializeField] Sprite spriteBlue;
    [SerializeField] Sprite spriteRed;
    [SerializeField] Sprite spritePurple;

    public void SetColor(SquareColor squareColor)
    {
        switch (squareColor)
        {
            case SquareColor.Yellow:
                deckDisplaySquare.sprite = spriteYellow;
                break;
            case SquareColor.Blue:
                deckDisplaySquare.sprite = spriteBlue;
                break;
            case SquareColor.Red:
                deckDisplaySquare.sprite = spriteRed;
                break;
            case SquareColor.Purple:
                deckDisplaySquare.sprite = spritePurple;
                break;
        }
    }

    public void SetColor(int squareColor)
    {
        switch (squareColor)
        {
            case 0:
                deckDisplaySquare.sprite = spriteYellow;
                break;
            case 1:
                deckDisplaySquare.sprite = spriteBlue;
                break;
            case 2:
                deckDisplaySquare.sprite = spriteRed;
                break;
            case 3:
                deckDisplaySquare.sprite = spritePurple;
                break;
        }
    }

    public void SetNumber(int number)
    {
        numberTMP.text = number.ToString() + "°³";
    }
}
