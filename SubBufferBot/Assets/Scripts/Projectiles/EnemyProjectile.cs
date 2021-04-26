using System;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    public static event Action<float> OnPlayerDamaged;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            OnPlayerDamaged?.Invoke(transform.position.x);
            ReturnToPool();
        }
    }
}