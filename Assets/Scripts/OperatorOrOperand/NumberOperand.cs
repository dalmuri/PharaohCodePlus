using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberOperand : OperatorOrOperand
{
    int number = 0;

    public void SetNumber(int num)
    {
        this.number = num;
        operTMP.text = num.ToString();
    }

    protected override void SetUpIntialState()
    {
        canDrag = false;
        if(parentOperator != null && parentOperator.operType != OperType.EqualSign)
            width = widthOperandLength;
        height = heightOperandLength;
        SetOrderFromParent();
    }

    protected override void SetSizeOfThis()
    {
        // Do Nothing!
    }

    protected override void AlignPosOfExpression()
    {
        // Do Nothing!
    }

    public override int Calculate()
    {
        CalculateManager.instance.numberOfUsedNumberOperand += 1;
        return number;
    }
}
