using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBonus : Bonus
{
    public override void apply()
    {
        game.SetActiveBonusWall();
    }
}
