using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootOperator : OperatorOrOperand
{
    protected override void SetUpIntialState()
    {
        width = widthIntervalBetweenChildAndEdge + widthIntervalBetweenUnaryOperatorAndChild + widthOperandLength + widthIntervalBetweenChildAndEdge;
        height = heightOperandLength + heightInterval;
        SetOrderFromParent();
    }

    protected override void SetSizeOfThis()
    {
        SetSize(widthIntervalBetweenChildAndEdge + widthIntervalBetweenUnaryOperatorAndChild + childrenOpers[0].width + widthIntervalBetweenChildAndEdge,
                childrenOpers[0].height + heightInterval);
    }

    protected override void AlignPosOfExpression()
    {
        float childWidth = childrenOpers[0].width;

        operTMP.transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge, 0, 0);
        GetComponent<BoxCollider2D>().offset = operTMP.transform.localPosition;
        childrenOpers[0].transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge + widthIntervalBetweenUnaryOperatorAndChild + childWidth / 2f, 0, 0);

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
        return true;
    }

    public override int Calculate()
    {
        int resultOfChild = childrenOpers[0].Calculate();
        float result = Mathf.Sqrt(resultOfChild);

        if (resultOfChild < 0)
        {
            CalculateManager.instance.existsError[(int)ErrorType.NegativeRoot] = true;
            return errorNum;
        }
        else if (result % 1f != 0)
        {
            CalculateManager.instance.existsError[(int)ErrorType.RootNotInt] = true;
            return errorNum;
        }

        return (int)result;
    }
}
