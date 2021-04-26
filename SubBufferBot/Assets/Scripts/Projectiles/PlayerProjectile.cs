using UnityEngine;

public class PlayerProjectile : Projectile
{
    public int charge;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(charge);
            ReturnToPool();
        }
    }
}