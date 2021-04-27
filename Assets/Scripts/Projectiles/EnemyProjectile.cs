using System;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    public static event Action<float, Enemy.Enemies> OnPlayerDamaged;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            OnPlayerDamaged?.Invoke(transform.position.x, Enemy.Enemies.LongRange);
            ReturnToPool();
        }
    }
}