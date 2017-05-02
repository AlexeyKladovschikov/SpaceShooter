using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BonusCategory
{
    Heal,
    Immune,
    FireRate
}

public interface IBonus
{
    BonusCategory Category { get; }
    void Reset(Vector2 position);
    void SetOnDisposeAction(Action<IBonus> onDispose);
    void Dispose();
    bool IsActive { get; }
    BonusConfig Config { get; }
}

public abstract class Bonus : MonoBehaviour, IBonus
{
    public BonusConfig Config { get; protected set; }
    public virtual BonusCategory Category { get; private set; }
    public bool IsActive { get; set; }

    private Action<IBonus> _onBonusFinished = (bonus) => { };
    private Rigidbody2D _rigidbody;

    void Awake()
    {
        Initialize();
    }

    void FixedUpdate()
    {
        FixedTick();
    }
    
    private void Initialize()
    {
        float ratioScale = Config.Size / GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        transform.localScale = new Vector3(ratioScale, ratioScale, 1);

        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionGameObject = collision.gameObject;

        if (collisionGameObject.CompareTag(Arena.PlayerTag))
        {
            OnDeath();
        }
    }

    public void Dispose()
    {
        _rigidbody.isKinematic = true;
        ColliderSetActive(false);
        IsActive = false;
        gameObject.SetActive(false);
        DOTween.Kill(transform);
        _onBonusFinished = (bonus) => { };

    }
    
    public void Reset(Vector2 position)
    {
        IsActive = true;
        _rigidbody.isKinematic = false;
        transform.localScale = Vector3.one;
        transform.position = position;
        ColliderSetActive(true);
        gameObject.SetActive(true);
    }

    private void ColliderSetActive(bool active)
    {
        GetComponent<Collider2D>().enabled = active;
    }

    public void FixedTick()
    {
        Move();
    }

    protected virtual void Move()
    {
        float positionY = transform.position.y - Config.Speed * Time.deltaTime;
        transform.position = new Vector2(transform.position.x, positionY);
    }


    private void OnDeath()
    {
        ColliderSetActive(false);
        transform.DOScale(Vector3.zero, 0.2f).Play().OnComplete(() => _onBonusFinished(this));
    }
    
    public void SetOnDisposeAction(Action<IBonus> onDispose)
    {
        _onBonusFinished = onDispose;
    }
}

