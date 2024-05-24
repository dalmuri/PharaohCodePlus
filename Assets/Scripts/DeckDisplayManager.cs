using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckDisplayManager : MonoBehaviour
{
    public static DeckDisplayManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    const float posX = -7.3f;
    const float posYOfFloor0 = -0.7f;
    const float intervalY = 1.25f;

    [SerializeField] GameObject deckDisplaySquarePrefab;

    DeckDisplaySquare[] deckDisplaySquares;

    private void SetUpDeckDisplay()
    {
        deckDisplaySquares = new DeckDisplaySquare[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject deckDisplaySquareObject = Instantiate(deckDisplaySquarePrefab, new Vector3(posX, posYOfFloor0 + intervalY * i, 0), Quaternion.identity);
            deckDisplaySquareObject.transform.SetParent(this.transform, false);
            deckDisplaySquares[i] = deckDisplaySquareObject.GetComponent<DeckDisplaySquare>();
            deckDisplaySquares[i].SetColor(i);
            deckDisplaySquares[i].GetComponent<Order>().SetOriginOrder(1);
        }
    }

    public void SetDisplayNumber(int floor, int number)
    {
        deckDisplaySquares[floor].SetNumber(number);
    }

    public Vector3 GetPosOfDeckDisplay(int floor)
    {
        return deckDisplaySquares[floor].transform.position;
    }

    void Start()
    {
        SetUpDeckDisplay();
    }
}
