using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        enemyKind = Enemies.Basic;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    void Update()
    {
        MoveTowardsPlayer();
    }

    protected override void Attack() {}
}