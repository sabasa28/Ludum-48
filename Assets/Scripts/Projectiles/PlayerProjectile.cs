using UnityEngine;

public class PlayerProjectile : Projectile
{
    public bool collided;
    public int charge;

    void OnEnable()
    {
        collided = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (!collided && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(charge);

            collided = true;
            ReturnToPool();
        }
    }
}