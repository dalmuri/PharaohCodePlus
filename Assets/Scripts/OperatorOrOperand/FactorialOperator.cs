using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorialOperator : OperatorOrOperand
{
    protected override void SetUpIntialState()
    {
        width = widthIntervalBetweenChildAndEdge + widthOperandLength + widthIntervalBetweenUnaryOperatorAndChild + widthIntervalBetweenChildAndEdge;
        height = heightOperandLength + heightInterval;
        SetOrderFromParent();
    }

    protected override void SetSizeOfThis()
    {
        SetSize(widthIntervalBetweenChildAndEdge + childrenOpers[0].width + widthIntervalBetweenUnaryOperatorAndChild + widthIntervalBetweenChildAndEdge,
                childrenOpers[0].height + heightInterval);
    }

    protected override void AlignPosOfExpression()
    {
        float childWidth = childrenOpers[0].width;

        childrenOpers[0].transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge + childWidth / 2f, 0, 0);
        operTMP.transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge + childWidth + widthIntervalBetweenUnaryOperatorAndChild, 0, 0);
        GetComponent<BoxCollider2D>().offset = operTMP.transform.localPosition;

        parenthesisGO1.transform.localPosition = childrenOpers[0].transform.localPosition;
        parenthesisGO2.transform.localPosition = childrenOpers[0].transform.localPosition;
    }

    protected override void SetWidthOfParentheses()
    {
        if (operType == OperType.Operand || operType == OperType.EqualSign)
            return;

        parenthesisGO1.GetComponent<RectTransform>().sizeDelta = new Vector2(childrenOpers[0].width + widthIntervalBetweenChildAndParenthesis * 2, height);
        parenthesisGO2.GetComponent<RectTransform>().sizeDelta = new Vector2(childrenOpers[0].width + widthIntervalBetweenChildAndParenthesis * 2, height);
    }

    protected override bool ShowsParentheses()
    {
        if (childrenOpers[0].operType == OperType.Operand)
            return false;
        else
            return true;
    }

    public override int Calculate()
    {
        int resultOfChild = childrenOpers[0].Calculate();

        if (resultOfChild < 0)
        {
            CalculateManager.instance.existsError[(int)ErrorType.NegativeFactorial] = true;
            return errorNum;
        }
        else if (resultOfChild > 12)
        {
            CalculateManager.instance.existsError[(int)ErrorType.FactorialOutOfRange] = true;
            return errorNum;
        }

        int result = 1;
        for (int i = 1; i <= resultOfChild; i++)
            result *= i;

        return result;
    }
}
