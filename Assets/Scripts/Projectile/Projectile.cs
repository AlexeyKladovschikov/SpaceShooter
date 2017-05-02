using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IProjectile
{
    ProjectileCategory Category { get; }
    int Damage { get; }
    void Reset(Vector2 position, Vector2 direction, LayerMask layer);
    void SetOnDisposeAction(Action<IProjectile> onDispose);
    void Dispose();
}

public enum ProjectileCategory
{
    Gun,
    Rocket
}

public abstract class Projectile : MonoBehaviour, IProjectile
{
    private ProjectileConfig _config;
    private Vector2 _direction;
    private float _lifeTime;
    private Action<IProjectile> _onProjectileFinished = (projectile) => { }; 

    void Awake()
    {
        Initialize();
    }

    [Zenject.Inject]
    private void Inject(ProjectileConfig config)
    {
        _config = config;
    }

    private void Initialize()
    {
        float ratioScaleH = _config.Height / GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        float ratioScaleW = _config.Widht / GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        transform.localScale = new Vector3(ratioScaleW, ratioScaleH, 1);
    }

    public void FixedUpdate()
    {
        if (_lifeTime <= 0)
        {
            _onProjectileFinished(this);
        }

        MoveTo(_direction);
        _lifeTime -= Time.deltaTime;
            
    }

    public abstract ProjectileCategory Category { get; }

    public int Damage
    {
        get { return _config.Damage; }
    }

    public void Reset(Vector2 position, Vector2 direction, LayerMask layer)
    {
        
        transform.position = position + direction * _config.Height / 2;
        _direction = direction;
        gameObject.layer = layer;
        _lifeTime = _config.Lifetime;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        gameObject.SetActive(true);
    }

    public void Dispose()
    {
        _lifeTime = 0f;
        _onProjectileFinished = (projectile) => { };
        gameObject.SetActive(false);
    }

    private void MoveTo(Vector2 direction)
    {
        float positionX = transform.position.x + direction.x * _config.Speed * Time.deltaTime;
        float positionY = transform.position.y + direction.y * _config.Speed * Time.deltaTime;

        transform.position = new Vector2(positionX, positionY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionGameObject = collision.gameObject;
        if (collisionGameObject.CompareTag(Arena.EnemyTag) || collisionGameObject.CompareTag(Arena.PlayerTag))
        {
            _lifeTime = 0f;
        }
    }

    public void SetOnDisposeAction(Action<IProjectile> onDispose)
    {
        _onProjectileFinished = onDispose;
    }

    public class Factory : Factory<IProjectile>
    {
    }
}
