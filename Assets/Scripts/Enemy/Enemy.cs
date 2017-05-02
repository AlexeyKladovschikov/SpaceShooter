using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public interface IEnemy
{
    EnemyCategory Category { get; }
    Vector2 Position { get; }
    void Reset(Vector2 position, int level);
    void SetOnDisposeAction(Action<IEnemy> onDispose);
    void Dispose();
    bool IsActive { get; }
}

public enum EnemyCategory
{
    Hardy,
    Speedy
}

public abstract class Enemy : MonoBehaviour , IEnemy
{
    [SerializeField]
    private EnemyBody _body;

    [SerializeField]
    private ParticleSystem _Explosion;
    
    protected EnemyConfig Config;
    protected float Speed;

    private Rigidbody2D _rigidbody;
    private Action<IEnemy> _onEnemyFinished = (enemy) => { };
    private ProjectilePool _projectilePool;

    private int _health;
    private float _fireDelay;

    void Awake ()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        FixedTick();
    }

    [Zenject.Inject]
    private void Inject(EnemyConfig config, ProjectilePool projectilePool)
    {
        Config = config;
        _projectilePool = projectilePool;
    }

    private void Initialize()
    {
        _body.Initialize(CollisionEnter, Config);

        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public abstract EnemyCategory Category { get; }
    public bool IsActive { get; set; }
    public Vector2 Position { get { return transform.position; }}

    public void CollisionEnter(Collision2D collision)
    {
        GameObject collisionGameObject = collision.gameObject;

        if (collisionGameObject.CompareTag(Arena.ProjectileTag))
        {
            IProjectile projectile = collisionGameObject.GetComponent<IProjectile>();
            AppyDamage(projectile.Damage);
        }
        else if (collisionGameObject.CompareTag(Arena.PlayerTag))
        {
            StartCoroutine(OnDeath(false));
        }
    }

    public void SetOnDisposeAction(Action<IEnemy> onDispose)
    {
        _onEnemyFinished = onDispose;
    }

    public void Dispose()
    {
        _rigidbody.isKinematic = true;
        ColliderSetActive(false);
        IsActive = false;
        gameObject.SetActive(false);
        DOTween.Kill(transform);
        _onEnemyFinished = (enemy) => { };
    }

    public void Reset(Vector2 position, int level)
    {
        transform.position = position;
        Speed = Config.Speed + level * Config.Speed * 0.1f;

        IsActive = true;
        _rigidbody.isKinematic = false;
        _health = Config.Health;
        transform.localScale = Vector3.one;
        ColliderSetActive(true);
        gameObject.SetActive(true);
        _fireDelay = Random.Range(0, 1f / Config.FireRate); 

    }

    private void ColliderSetActive(bool active)
    {
        _body.GetComponent<Collider2D>().enabled = active;
    }

    public void FixedTick()
    {
        if (_health <= 0) return;
        Move();
        FireTick();
    }

    protected virtual void Move()
    {
        float positionY = transform.position.y - Speed * Time.deltaTime;
        float positionX = transform.position.x + Mathf.Sin(positionY) * Time.deltaTime * Speed;

        transform.position = new Vector2(positionX, positionY);
    }
    
    private IEnumerator OnDeath(bool kill)
    {
        ColliderSetActive(false);
        transform.DOScale(Vector3.zero, Config.DeathTime).Play();
        _Explosion.Play();

        if (kill)
        {
            EventManager.OnEnemyDied(this);
        }

        yield return new WaitForSeconds(_Explosion.main.duration);
        _onEnemyFinished(this);
    }

    private void AppyDamage(int damage)
    {
        _health -= damage;

        _body.GetComponent<SpriteRenderer>().color = Color.white;
        _body.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.1f).SetLoops(3, LoopType.Yoyo).Play().From();

        if (_health <= 0)
        {
            StartCoroutine(OnDeath(true));
        }
    }

    private void FireTick()
    {
        _fireDelay -= Time.deltaTime;
        if (_fireDelay < 0)
        {
            IProjectile projectile = _projectilePool.TakeProjectile(Config.Projectile);
            projectile.Reset(transform.position, -transform.up, LayerMask.NameToLayer(Arena.EnemyProjectileLayer));

            _fireDelay = 1f / Config.FireRate;
        }
    }
    
   
}
