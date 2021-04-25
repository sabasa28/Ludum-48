using UnityEngine;

public class PlayerProjectile : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("enemy damaged");

            ReturnToPool();
        }
    }
}