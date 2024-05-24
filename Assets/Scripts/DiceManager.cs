using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] int[] diceMaxNum;

    [SerializeField] GameObject calculationFieldGO;
    [SerializeField] GameObject numberOperandPrefab;

    NumberOperand[] dice;

    const int maxNumberOfDice = 4;
    readonly Vector3 spawnPos = new Vector3(5.5f, -4f, 0f);
    const float widthIntervalBetweenDice = 1f;

    const float diceMovingDelay = 0.3f;
    bool[] isRollingDiceEnded;
    readonly WaitForSeconds diceNumberChangeDelay = new WaitForSeconds(0.05f);
    readonly WaitForSeconds diceRollingTime = new WaitForSeconds(1.2f);
    readonly WaitForSeconds diceRolledInterval = new WaitForSeconds(0.2f);

    public static event Action RollDiceEvent;

    int _numberOfDice = 3;
    int numberOfDice
    {
        get => _numberOfDice;
        set { if (value > maxNumberOfDice) _numberOfDice = maxNumberOfDice; }
    }

    void SetUpDice()
    {
        numberOfDice = diceMaxNum.Length;

        dice = new NumberOperand[numberOfDice];
        for (int i = 0; i < numberOfDice; i++)
        {
            GameObject numberOperandObject = Instantiate(numberOperandPrefab, Vector3.zero, Quaternion.identity);
            numberOperandObject.transform.SetParent(calculationFieldGO.transform, false);
            numberOperandObject.transform.position = spawnPos + new Vector3(widthIntervalBetweenDice * i, 0f, 0f);

            dice[i] = numberOperandObject.GetComponent<NumberOperand>();
            dice[i].canDrag = false;
        }

        isRollingDiceEnded = new bool[numberOfDice];
    }

    public void MoveDice(params NumberOperand[] numberOperands)
    {
        for (int i = 0; i < numberOperands.Length; i++)
        {
            for (int j = 0; j < dice.Length; j++)
            {
                if(numberOperands[i] == dice[j])
                {
                    numberOperands[i].MoveTransform(spawnPos + new Vector3(widthIntervalBetweenDice * j, 0f, 0f), true, diceMovingDelay);
                    break;
                }
            }
        }
    }

    public IEnumerator RollDiceCo()
    {
        for (int i = 0; i < numberOfDice; i++)
        {
            dice[i].canDrag = false;
            isRollingDiceEnded[i] = false;
        }
        RollDiceEvent?.Invoke();
        MoveDice(dice);
        yield return new WaitForSeconds(diceMovingDelay);

        for (int i = 0; i < numberOfDice; i++)
        {
            StartCoroutine(RollCo(i));
        }
        yield return diceRollingTime;

        for (int i = 0; i < numberOfDice; i++)
        {
            isRollingDiceEnded[i] = true;
            dice[i].canDrag = true;
            yield return diceRolledInterval;
        }
    }

    IEnumerator RollCo(int index)
    {
        while (!isRollingDiceEnded[index])
        {
            dice[index].SetNumber(Random.Range(1, diceMaxNum[index] + 1));
            yield return diceNumberChangeDelay;
        }
    }

    private void Start()
    {
        SetUpDice();
    }
}
