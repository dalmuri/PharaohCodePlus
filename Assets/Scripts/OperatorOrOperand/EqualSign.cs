using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualSign : OperatorOrOperand
{
    protected override void SetUpIntialState()
    {
        canDrag = false;
        width = widthIntervalBetweenChildAndEdge + widthOperandLength + widthBinaryOperator + widthOperandLength + widthIntervalBetweenChildAndEdge;
        height = heightOperandLength + heightInterval;
        SetOrderFromParent();
    }

    protected override void SetSizeOfThis()
    {
        SetSize(widthIntervalBetweenChildAndEdge + childrenOpers[0].width + widthBinaryOperator + childrenOpers[1].width + widthIntervalBetweenChildAndEdge,
                max(childrenOpers[0].height, childrenOpers[1].height) + heightInterval);

        float max(float num1, float num2)
        {
            if (num1 > num2) return num1;
            else return num2;
        }
    }

    protected override void AlignPosOfExpression()
    {
        float childWidth1 = childrenOpers[0].width;
        float childWidth2 = childrenOpers[1].width;

        childrenOpers[0].transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge + childWidth1 / 2f, 0, 0);
        operTMP.transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge + childWidth1 + widthBinaryOperator / 2f, 0, 0);
        GetComponent<BoxCollider2D>().offset = operTMP.transform.localPosition;
        childrenOpers[1].transform.localPosition = new Vector3(-width / 2f + widthIntervalBetweenChildAndEdge + childWidth1 + widthBinaryOperator + childWidth2 / 2f, 0, 0);
    }

    public override int Calculate()
    {
        int result = childrenOpers[0].Calculate();
        if(result > 999 || result < -999)
        {
            CalculateManager.instance.existsError[(int)ErrorType.ResultOver999] = true;
            return errorNum;
        }

        return result;
    }
}
