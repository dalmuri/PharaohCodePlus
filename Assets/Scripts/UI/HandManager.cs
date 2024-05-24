using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] GameObject handNumberSquarePrefab;
    [SerializeField] GameObject handScrollGO;
    [SerializeField] GameObject canvas;
    [SerializeField] Renderer hoverBox;
    [SerializeField] ProgressButton progressButton;

    List<HandNumberSquare> handNumberSquares;

    HandNumberSquare selectedHandNumberSquare;

    void SetUpHand()
    {
        handNumberSquares = new List<HandNumberSquare>(10);
    }

    public void AddHand(NumberInfo numberInfo)
    {
        GameObject handNumberSquareObject = Instantiate(handNumberSquarePrefab, Vector3.zero, Quaternion.identity, canvas.transform);
        handNumberSquareObject.transform.SetParent(handScrollGO.transform, false);
        HandNumberSquare handNumberSquare = handNumberSquareObject.GetComponent<HandNumberSquare>();
        handNumberSquare.SetUp(numberInfo);

        handNumberSquares.Add(handNumberSquare);
        progressButton.CheckProgressBtnState();
    }

    public void RemoveHandNumberSquare(HandNumberSquare handNumberSquare)
    {
        handNumberSquares.Remove(handNumberSquare);
        Destroy(handNumberSquare.gameObject);
        progressButton.CheckProgressBtnState();
    }    

    public void SelectHandNumberSquare(HandNumberSquare handNumberSquare)
    {
        selectedHandNumberSquare = handNumberSquare;
        CheckCalculation();
    }

    void CheckCalculation()
    {
        if(selectedHandNumberSquare != null && selectedHandNumberSquare.number == CalculateManager.instance.result)
        {
            if(CalculateManager.instance.numberOfUsedNumberOperand > 1)
            {
                ScoreManager.instance.AddScore(selectedHandNumberSquare.point);
                RemoveHandNumberSquare(selectedHandNumberSquare);
            }
            else
            {
                CalculateManager.instance.existsError[(int)ErrorType.UsedUnder2Numbers] = true;
                CalculateManager.instance.ShowCalculationErrorMessage();
            }
        }
    }

    public int GetNumberOfHand()
    {
        if (handNumberSquares == null)
            return 0;

        return handNumberSquares.Count;
    }

    public void CheckHoverBoxVisible(bool visible)
    {
        if (visible)
            hoverBox.enabled = true;
        else
            hoverBox.enabled = false;
    }
    
    public void GiveUp()
    {
        int point = 0;
        while (handNumberSquares.Count > 0)
        {
            point -= handNumberSquares[0].point;
            RemoveHandNumberSquare(handNumberSquares[0]);
        }

        ScoreManager.instance.AddScore(point);
    }

    void Start()
    {
        SetUpHand();
    }
}
