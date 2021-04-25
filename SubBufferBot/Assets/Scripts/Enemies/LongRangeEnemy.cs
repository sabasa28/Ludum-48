using System;
using UnityEngine;

public class LongRangeEnemy : Enemy
{
    [SerializeField] float projectileSpeed = 2.0f;

    public static event Action<float, Vector2, Vector2> OnShoot;

    void Update()
    {
        MoveTowardsPlayer();

        if (PlayerInRange() && attackReady) Attack();
    }

    protected override void Attack()
    {
        OnShoot?.Invoke(projectileSpeed, transform.position, GetPlayerDirection());

        StartCoroutine(CoolDown());
    }
}