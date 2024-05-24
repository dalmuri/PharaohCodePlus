using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivisionOperator : ArithmeticOperator
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
        int resultOfChild1 = childrenOpers[0].Calculate();
        int resultOfChild2 = childrenOpers[1].Calculate();

        if(resultOfChild2 == 0)
        {
            CalculateManager.instance.existsError[(int)ErrorType.DivideZero] = true;
            return errorNum;
        }
        else if(resultOfChild1 % resultOfChild2 != 0)
        {
            CalculateManager.instance.existsError[(int)ErrorType.DivisionNotInt] = true;
            return errorNum;
        }

        return childrenOpers[0].Calculate() / childrenOpers[1].Calculate();
    }
}
