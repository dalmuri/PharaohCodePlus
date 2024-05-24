using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionOperator : ArithmeticOperator
{
    protected override bool ShowsParentheses()
    {
        if (parentOperator is MultiplicationOperator || parentOperator is DivisionOperator || (parentOperator is SubtractionOperator && childNum == 1))
            return true;
        else
            return false;
    }

    public override int Calculate()
    {
        return childrenOpers[0].Calculate() + childrenOpers[1].Calculate();
    }
}
