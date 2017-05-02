using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    ProjectileCategory Category { get; }
    bool HasFire { get; }
    void StartFire();
    void EndFire();
    void BoostFireRate(float modificator, float duration);
}

public abstract class Weapon : MonoBehaviour, IWeapon
{
    public virtual ProjectileCategory Category { get; private set; }
    public bool HasFire { get; private set; }

    private float _fireDelay;
    private float _fireRate;

    protected ProjectilePool ProjectilePool;
    protected WeaponConfig Config;

    [Zenject.Inject]
    private void Inject(WeaponConfig config, ProjectilePool projectilePool)
    {
        Config = config;
        ProjectilePool = projectilePool;
        _fireRate = Config.FireRate;
    }

    private void Initialize()
    {
        _fireDelay = 0;
        HasFire = false;
    }

    public void StartFire()
    {
        HasFire = true;
    }

    public void EndFire()
    {
        HasFire = false;
    }

    public void BoostFireRate(float modificator, float duration)
    {
        _fireDelay = 0f;
        _fireRate = Config.FireRate * modificator;
        StartCoroutine(StopBoost(duration));
    }

    private IEnumerator StopBoost(float duration)
    {
        yield return new WaitForSeconds(duration);
        _fireRate = Config.FireRate;
    }

    private void FixedUpdate()
    {
        if (HasFire)
        {
            _fireDelay -= Time.deltaTime;
            if (_fireDelay < 0)
            {
                PlaceProjectiles();
                _fireDelay = 1f / _fireRate;

            }
        }
    }

    protected virtual void PlaceProjectiles()
    {
        IProjectile projectile = ProjectilePool.TakeProjectile(Config.Category);
        projectile.Reset(transform.position, transform.up, LayerMask.NameToLayer(Arena.PlayerProjectileLayer));
    }

    
}
