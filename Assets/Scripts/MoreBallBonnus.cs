using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreBallBonnus : Bonus
{
    public int add = 1;

    public override void apply()
    {
        game.bulletCount = Mathf.Min(game.bulletCount + add, 100);
        for (int i = 0; i < add; i++)
        {
            game.InstantiateBullet();
        }
    }
}
