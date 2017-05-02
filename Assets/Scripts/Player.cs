using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public interface ITargetable
{
    Vector2 Position { get; }
}

public class Player : MonoBehaviour, ITargetable
{
    [SerializeField]
    private ParticleSystem _explosion;

    private PlayerConfig _config;
    private bool _isImmunable;
    private float _immuneTimeLeft;
    private int _health;
    private List<IWeapon> _weapons;
     
    public bool HasFire { get; private set; }
    private int _selectedWeaponIndex;

    private int Health
    {
        get { return _health;}
        set
        {
            _health = value;
            if (_health > 0)
            {
                EventManager.OnPlayerHealthChanged(_health);
            }
        }
    }

    public Vector2 Position
    {
        get { return transform.position; } 
    }

    private void Start()
    {
        Initialize();
    }

    [Zenject.Inject]
    private void Inject(PlayerConfig config, List<IWeapon> weapons)
    {
        _config = config;
        _weapons = weapons;
    }

    private void Initialize()
    {
        Health = _config.Hp;
        _isImmunable = false;
        _selectedWeaponIndex = 0;
        float ratioScale = _config.Size / GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        transform.localScale = new Vector3(ratioScale, ratioScale, 1);
        
    }

    private void Update()
    {
        if (_isImmunable)
        {
            if (_immuneTimeLeft < 0)
            {
                RemoveImmunable();
            }

            _immuneTimeLeft -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionGameObject = collision.gameObject;

        if (collisionGameObject.CompareTag(Arena.EnemyTag) || collisionGameObject.CompareTag(Arena.ProjectileTag))
        {
            ApplyDamage();
        }
        if (collisionGameObject.CompareTag(Arena.BonusTag))
        {
            ApplyBonus(collisionGameObject.GetComponent<IBonus>());
        }
    }

    private IWeapon SelectedWeapon
    {
        get
        {
            return _weapons[_selectedWeaponIndex];
        }
    }

    private void ApplyDamage()
    {
        if (_isImmunable)
        {
            return;
        }

        Health--;

        if (Health <= 0)
        {
            Die();
            return;
        }

        SetImmunable(_config.ImmuneTime);
    }

    private void SetImmunable(float duration)
    {
        _isImmunable = true;
        _immuneTimeLeft = duration;
        GetComponent<SpriteRenderer>().DOColor(Color.white, 0f);

        //Start blinking
        GetComponent<SpriteRenderer>().DOColor(Color.red, 0.1f)
            .SetLoops(Mathf.CeilToInt(duration / 0.1f), LoopType.Yoyo)
            .OnComplete(() => GetComponent<SpriteRenderer>().color = Color.white);
    }

    private void RemoveImmunable()
    {
        _isImmunable = false;
        DOTween.Kill(GetComponent<SpriteRenderer>(), true);
    }

    public void StartFire()
    {
        SelectedWeapon.StartFire();
    }

    public void EndFire()
    {
        SelectedWeapon.EndFire();
    }

    public void PrevWeapon()
    {
        ChangeWeapon(-1);
    }

    public void NextWeapon()
    {
        ChangeWeapon(1);
    }

    private void ChangeWeapon(int step)
    {
        bool hasFire = SelectedWeapon.HasFire;
        if (hasFire)
        {
            SelectedWeapon.EndFire();
        }

        _selectedWeaponIndex = (_selectedWeaponIndex + step) % _weapons.Count;

        if (_selectedWeaponIndex < 0)
        {
            _selectedWeaponIndex += _weapons.Count;
        }

        if (hasFire)
        {
            SelectedWeapon.StartFire();
        }
    }

    public void MoveTo(float dx)
    {
        float newXpos = transform.position.x + dx * _config.Speed * Time.deltaTime;

        transform.position = new Vector2(newXpos, transform.position.y);
    }
    
    private void Die()
    {
        GetComponent<Collider2D>().enabled = false;
        SelectedWeapon.EndFire();
        EventManager.OnPlayerDied();
        transform.DOScale(Vector3.zero, 0.5f).Play();
        _explosion.Play();
    }

    private void ApplyBonus(IBonus bonus)
    {
        switch (bonus.Category)
        {
            case BonusCategory.FireRate:
                FireRateBonusConfig fireRateConfig = (FireRateBonusConfig)bonus.Config;
                foreach (var weapon in _weapons)
                {
                    weapon.BoostFireRate(fireRateConfig.Modificator, fireRateConfig.Duration);
                }
                break;

            case BonusCategory.Heal:
                HealBonusConfig healConfig = (HealBonusConfig)bonus.Config;
                Health += healConfig.Heal;
                break;

            case BonusCategory.Immune:
                ImmuneBonusConfig immuneConfig = (ImmuneBonusConfig)bonus.Config;
                SetImmunable(immuneConfig.Duration);
                break;
        }
    }
}
