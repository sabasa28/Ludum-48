using UnityEngine;

public class EnemyProjectile : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("player damaged");

            ReturnToPool();
        }
    }
}