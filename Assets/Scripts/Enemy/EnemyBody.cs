using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class EnemyBody : MonoBehaviour
{
    private Action<Collision2D> _onCollisionEnter = (collision) => { };

    public Bounds BodyBounds { get; private set; }

    public void Initialize(Action<Collision2D> onCollisionEnter, EnemyConfig config)
    {
        _onCollisionEnter = onCollisionEnter;

        float ratioScale = config.Size / GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        transform.localScale = new Vector3(ratioScale, ratioScale, 1);
        transform.localPosition = Vector3.zero;
        BodyBounds = GetComponent<Collider2D>().bounds;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _onCollisionEnter(collision);
    }


}
