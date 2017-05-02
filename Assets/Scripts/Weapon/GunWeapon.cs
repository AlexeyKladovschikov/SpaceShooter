using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GunWeapon : Weapon
{
    [SerializeField]
    private Transform LeftPosition;

    [SerializeField]
    private Transform RightPosition;

    public override ProjectileCategory Category
    {
        get
        {
            return ProjectileCategory.Gun;
        }
    }

    protected override void PlaceProjectiles()
    {
        IProjectile projectileLeft = ProjectilePool.TakeProjectile(Config.Category);
        projectileLeft.Reset(LeftPosition.position, transform.up, LayerMask.NameToLayer(Arena.PlayerProjectileLayer));
        IProjectile projectileRight = ProjectilePool.TakeProjectile(Config.Category);
        projectileRight.Reset(RightPosition.position, transform.up, LayerMask.NameToLayer(Arena.PlayerProjectileLayer));

    }
}

