using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GunProjectile : Projectile
{
    public override ProjectileCategory Category
    {
        get { return ProjectileCategory.Gun; }
    }
}

