using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelConfig
{
    public PlayerConfig Player;
    public EnemiesConfig Enemies;
    public ProjectilesConfig Projectiles;
    public BonusesConfig Bonuses;
    public ArenaConfig Arena;
    public CameraConfig Camera;
}

[Serializable]
public class PlayerConfig
{
    public float Size;
    public float Speed;
    public int Hp;
    public float ImmuneTime;
    public WeaponConfig RocketWeaponConfig;
    public WeaponConfig GunWeaponConfig;

}

[Serializable]
public class EnemiesConfig
{
    public EnemyConfig SpeedyEnemy;
    public EnemyConfig HardyEnemy;
}

[Serializable]
public class ProjectilesConfig
{
    public ProjectileConfig GunProjectile;
    public ProjectileConfig RocketProjectile;
}

[Serializable]
public class EnemyConfig
{
    public int Health;
    public float FireRate;
    public ProjectileCategory Projectile;
    public float Speed;
    public float Size;
    public float DeathTime;
    public int PoolSize;
}

[Serializable]
public class WeaponConfig
{
    public float FireRate;
    public ProjectileCategory Category;
}

[Serializable]
public class ProjectileConfig
{
    public float Widht;
    public float Height;
    public float Speed;
    public int Damage;
    public float Lifetime;
    public int PoolSize;
}

[Serializable]
public class ArenaConfig
{
    public float Width;
    public float Height;
    public float EnemySpawnDelay;
    public float LevelUpTimer;
}

[Serializable]
public class CameraConfig
{
    public float BorderMove;
}

[Serializable]
public class BonusesConfig
{
    public HealBonusConfig HealConfig;
    public ImmuneBonusConfig ImmuneConfig;
    public FireRateBonusConfig FireRateConfig;
}

[Serializable]
public class BonusConfig
{
    public float Speed;
    public float Size;
}

[Serializable]
public class HealBonusConfig: BonusConfig
{
    public int Heal;
}

[Serializable]
public class ImmuneBonusConfig : BonusConfig
{
    public float Duration;
}

[Serializable]
public class FireRateBonusConfig : BonusConfig
{
    public float Modificator;
    public float Duration;
}


