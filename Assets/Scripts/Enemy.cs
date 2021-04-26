using System;
using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;

    protected bool attackReady = true;

    [SerializeField] int level = 1;

    [SerializeField] protected float movementSpeed = 1.0f;
    float positionY = -1.61f;
    bool grounded = false;

    [Header("Attack: ")]
    [SerializeField] protected float range = 5.0f;
    [SerializeField] protected float attackCoolDown = 2.0f;

    [HideInInspector] public Transform playerTransform;

    public static event Action<Enemy> OnDeath;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!grounded && collision.CompareTag("Floor"))
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0.0f;

            Vector2 position = transform.position;
            position.y = positionY;
            transform.position = position;

            grounded = true;
        }
    }

    protected abstract void Attack();

    protected void MoveTowardsPlayer()
    {
        Vector2 position = transform.position;
        position += GetPlayerDirection() * movementSpeed * Time.deltaTime;
        transform.position = position;
    }

    protected bool PlayerInRange()
    {
        return Vector2.Distance(playerTransform.position, transform.position) <= range;
    }

    protected Vector2 GetPlayerDirection()
    {
        return playerTransform.position.x > transform.position.x ? Vector2.right : -Vector2.right;
    }

    protected void Die()
    {
        OnDeath?.Invoke(this);
    }

    public void TakeDamage(int attackCharge)
    {
        if (attackCharge >= level) Die();
    }

    protected IEnumerator CoolDown()
    {
        attackReady = false;

        yield return new WaitForSeconds(attackCoolDown);

        attackReady = true;
    }
}