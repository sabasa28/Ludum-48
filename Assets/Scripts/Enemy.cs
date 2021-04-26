using System;
using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected BoxCollider2D raycastReceiver;
    protected Rigidbody2D rb;
    protected BoxCollider2D mainColl;
    protected AudioManager audioManager;
    Animator anim;
    SpriteRenderer sr;

    protected bool attackReady = true;
    public bool dead = false;

    [SerializeField] int level = 1;

    [SerializeField] protected float movementSpeed = 1.0f;
    float positionY = -1.61f;
    bool grounded = false;

    bool animatingDeath = false;
    [SerializeField] float minDistToPlayerToFlip = 0.0f;

    [Header("Attack: ")]
    [SerializeField] protected float range = 5.0f;
    [SerializeField] protected float attackCoolDown = 2.0f;

    [HideInInspector] public Transform playerTransform;

    public static event Action<Enemy> OnDeath;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainColl = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        audioManager = AudioManager.Get();
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
        if (animatingDeath) return;
        Vector2 position = transform.position;
        Vector2 playerDir = GetPlayerDirection();
        position += playerDir * movementSpeed * Time.deltaTime;
        transform.position = position;
        if (Mathf.Abs(transform.position.x - playerTransform.position.x) > minDistToPlayerToFlip)
        {
            if (playerDir.x > 0 && sr.flipX) sr.flipX = false;
            else if (playerDir.x < 0 && !sr.flipX) sr.flipX = true;
        }
    }

    protected bool PlayerInRange()
    {
        return Vector2.Distance(playerTransform.position, transform.position) <= range;
    }

    protected Vector2 GetPlayerDirection()
    {
        return playerTransform.position.x > transform.position.x ? Vector2.right : -Vector2.right;
    }

    protected void WaitForAnimation()
    {
        audioManager.PlaySound(AudioManager.Sounds.EnemyDeath);

        dead = true;
        mainColl.enabled = false;
        raycastReceiver.enabled = false;
        animatingDeath = true;
        anim.SetTrigger("Dead");
    }

    protected void Die()
    {
        OnDeath?.Invoke(this);
    }

    public void TakeDamage(int attackCharge)
    {
        if (attackCharge >= level) WaitForAnimation();
    }

    public void Squished(int attackCharge)
    {
        TakeDamage(attackCharge);
        anim.SetTrigger("Smashed");
    }

    protected IEnumerator CoolDown()
    {
        attackReady = false;

        yield return new WaitForSeconds(attackCoolDown);

        attackReady = true;
    }
}