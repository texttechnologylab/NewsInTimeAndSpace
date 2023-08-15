using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusMinusButton : UIButton
{
    public IntegerMenu integerMenu;
    public PlusMinus plusMinus;

    public enum PlusMinus
    {
        Plus,
        Minus
    }

    public override void onRaycastInteraction()
    {
        if (plusMinus == PlusMinus.Plus)
        {
            integerMenu.increaseValue();
        }
        else if (plusMinus == PlusMinus.Minus)
        {
            integerMenu.decreaseValue();
        }
    }
}
