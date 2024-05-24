using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExponentiationOperator : OperatorOrOperand
{
    protected override void SetUpIntialState()
    {
        width = widthIntervalBetweenChildAndEdge + widthOperandLength + widthIntervalBetweenExponentiationOperatorChildren + widthOperandLength + widthIntervalBetweenChildAndEdge;
        height = heightInterval / 2f + heightOperandLength / 2f + heightOperandLength / 2f + heightMinimumIntervalBetweenChildren + heightOperandLength / 2f + heightInterval / 2f;
        SetOrderFromParent();
    }

    protected override void SetSizeOfThis()
    {
        SetSize(widthIntervalBetweenChildAndEdge + childrenOpers[0].width + widthIntervalBetweenExponentiationOperatorChildren + childrenOpers[1].width + widthIntervalBetweenChildAndEdge,
                heightInterval / 2f + childrenOpers[0].height / 2f + max(childrenOpers[0].height / 2f, childrenOpers[1].height / 2f) + heightMinimumIntervalBetweenChildren + childrenOpers[1].height / 2f + heightInterval / 2f);

        float max(float num1, float num2) // local method max
        {
            if (num1 > num2) return num1;
            else return num2;
        }
    }

    protected override void AlignPosOfExpression()
    {
        float childWidth1 = childrenOpers[0].width;
        float childWidth2 = childrenOpers[1].width;

        float childHeight1 = childrenOpers[0].height;
        float childHeight2 = childrenOpers[1].height;

        childrenOpers[0].transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge + childWidth1 / 2f,
                                                               -height / 2f + heightInterval / 2f + childHeight1 / 2f,
                                                               0);
        childrenOpers[1].transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge + childWidth1 + widthIntervalBetweenExponentiationOperatorChildren + childWidth2 / 2f,
                                                               height / 2f - heightInterval / 2f - childHeight2 / 2f,
                                                               0);
        operTMP.transform.localPosition = new Vector3(childrenOpers[1].transform.localPosition.x,
                                                      (-childrenOpers[1].height - heightInterval / 2f) / 2f,
                                                      0);
        GetComponent<BoxCollider2D>().offset = operTMP.transform.localPosition;
        GetComponent<BoxCollider2D>().size = new Vector2(childrenOpers[1].width, height - childrenOpers[1].height - heightInterval);

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
        int resultOfChild1 = childrenOpers[0].Calculate();
        int resultOfChild2 = childrenOpers[1].Calculate();
        if (resultOfChild2 < 0)
        {
            CalculateManager.instance.existsError[(int)ErrorType.NegativeExponent] = true;
            return errorNum;
        }
        else if(resultOfChild1 == 0 && resultOfChild2 == 0)
        {
            CalculateManager.instance.existsError[(int)ErrorType.ZeroToThePowerOfZero] = true;
            return errorNum;
        }

        return (int)Mathf.Pow(resultOfChild1, resultOfChild2);
    }
}
