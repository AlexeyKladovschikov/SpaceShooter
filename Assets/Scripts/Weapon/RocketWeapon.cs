using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class RocketWeapon : Weapon
{
    public override ProjectileCategory Category
    {
        get
        {
            return ProjectileCategory.Rocket;
        }
    }
}

