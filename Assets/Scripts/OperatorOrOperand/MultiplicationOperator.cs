using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplicationOperator : ArithmeticOperator
{
    protected override bool ShowsParentheses()
    {
        if (parentOperator is DivisionOperator && childNum == 1)
            return true;
        else
            return false;
    }

    public override int Calculate()
    {
        return childrenOpers[0].Calculate() * childrenOpers[1].Calculate();
    }
}
