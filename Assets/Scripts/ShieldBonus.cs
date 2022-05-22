using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBonus : Bonus
{
    public override void apply()
    {
        game.SetActiveBonusWall();
    }
}
