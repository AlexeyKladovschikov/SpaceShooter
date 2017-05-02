using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class HardyEnemy: Enemy
{
    protected override void Move()
    {
        float positionY = transform.position.y - Speed * Time.deltaTime;

        transform.position = new Vector2(transform.position.x, positionY);
    }

    public override EnemyCategory Category
    {
        get { return EnemyCategory.Hardy; }
    }
}

