using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SpeedyEnemy: Enemy
{
    public override EnemyCategory Category
    {
        get { return EnemyCategory.Speedy;}
    }
}

