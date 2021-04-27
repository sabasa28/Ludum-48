using System;
using UnityEngine;

public class LongRangeEnemy : Enemy
{
    [SerializeField] float projectileSpeed = 2.0f;

    public static event Action<float, Vector2, Vector2> OnShoot;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    void Update()
    {

        MoveTowardsPlayer();

        if (PlayerInRange() && attackReady) Attack();
    }

    protected override void Attack()
    {
        if (dead) return;

        OnShoot?.Invoke(projectileSpeed, transform.position, (playerTransform.position - transform.position).normalized);
        anim.SetTrigger("Attacking");
        audioManager.PlaySound(AudioManager.Sounds.EnemyProjectile);
        StartCoroutine(CoolDown());
    }
}