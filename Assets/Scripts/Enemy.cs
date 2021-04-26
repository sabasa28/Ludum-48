using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected bool attackReady = true;

    [SerializeField] int level = 1;

    [SerializeField] protected float movementSpeed = 1.0f;

    [Header("Attack: ")]
    [SerializeField] protected float range = 5.0f;
    [SerializeField] protected float attackCoolDown = 2.0f;

    [HideInInspector] public Transform playerTransform;

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

    public void TakeDamage(int attackCharge)
    {
        if (attackCharge >= level) Destroy(gameObject);
    }

    protected IEnumerator CoolDown()
    {
        attackReady = false;

        yield return new WaitForSeconds(attackCoolDown);

        attackReady = true;
    }
}