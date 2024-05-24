using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OperatorSpawnerBtn : MonoBehaviour
{
    [SerializeField] TMP_Text operatorTMP;

    OperatorType operatorType;

    public void SetUp(OperatorType operatorType)
    {
        this.operatorType = operatorType;
        operatorTMP.fontSize = 60;
        operatorTMP.alignment = TextAlignmentOptions.Midline;

        switch (operatorType)
        {
            case OperatorType.Addition:
                operatorTMP.text = "+";
                break;
            case OperatorType.Subtraction:
                operatorTMP.alignment = TextAlignmentOptions.Capline;
                operatorTMP.text = "-";
                break;
            case OperatorType.Multiplication:
                operatorTMP.text = "¡¿";
                break;
            case OperatorType.Division:
                operatorTMP.text = "¡À";
                break;
            case OperatorType.Exponentiation:
                operatorTMP.fontSize = 24;
                operatorTMP.text = "Exponent";
                break;
            case OperatorType.Root:
                operatorTMP.fontSize = 40;
                operatorTMP.text = "¡î";
                break;
            case OperatorType.Factorial:
                operatorTMP.fontSize = 45;
                operatorTMP.text = "!";
                break;
        }
    }

    public void OnClick()
    {
        CalculateManager.instance.SpawnCalculationOperator(operatorType);
    }
}
