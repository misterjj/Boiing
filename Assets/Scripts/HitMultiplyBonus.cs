using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMultiplyBonus : Bonus
{
    public int multiply = 2;

    public override void apply()
    {
        game.SetNextHitValue(game.GetNextHitValue() * multiply);
    }
}
