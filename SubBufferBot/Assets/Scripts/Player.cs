﻿using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    [Header("Health: ")]
    [SerializeField] int lives = 3;
    public int Lives { private set { lives = value; } get { return lives; } }

    [SerializeField] float invincibilityDuration = 3.0f;
    bool invincible = false;
    bool exitedInvisibilityFrames = false;

    [Header("Movement: ")]
    [SerializeField] float speedX = 0.0f;
    Vector2 movement;

    bool lookingRight; //right==true  left==false
    bool lookingUp; //up==true  down==false     en verdad no es up, sino mirando hacia adelante

    [SerializeField] float drag;
    float extraHorizontalPhysics;

    [SerializeField] float jumpBaseForce;
    [SerializeField] float jumpForcePerChargeLvl;
    [SerializeField] float pushWhenHurtForce;

    [SerializeField] bool startsGrounded;
    bool isGrounded = false;

    bool aboveEnemy = false;

    bool onHurtAnim = false;
    bool frameSkipped = false;

    [Header("Projectiles: ")]
    [SerializeField] float projectileSpeed = 2.0f;
    [SerializeField] float chargingSpeed = 1.0f;
    int lastShotCharge;
    bool isCharging = false;

    public static event Action<int, float, Vector2, Vector2> OnShoot;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        EnemyProjectile.OnPlayerDamaged += TakeDamage;
        ChargedAttack.OnPlayerDamaged += TakeDamage;
    }

    void Start()
    {
        lookingRight = true;
        lookingUp = true;
        isGrounded = startsGrounded;
    }

    void FixedUpdate()
    {
        FlipIfNeeded();

        if (onHurtAnim) 
        {
            if (frameSkipped)
            {
                if (rb.velocity == Vector2.zero) onHurtAnim = false;
                frameSkipped = false;
            }
            else
                frameSkipped = true;
        }
        else if (!isCharging)
        {
            rb.velocity = new Vector2(movement.x + extraHorizontalPhysics, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(extraHorizontalPhysics, rb.velocity.y);
        }

        if (extraHorizontalPhysics != 0.0f) extraHorizontalPhysics *= drag;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!exitedInvisibilityFrames) CheckCollisionWithEnemy(collision);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (exitedInvisibilityFrames)
        {
            CheckCollisionWithEnemy(collision);

            exitedInvisibilityFrames = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor")) isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor")) isGrounded = false;
    }

    void Update()
    {
        movement = Vector2.zero;
        movement.x = (Input.GetAxis("Horizontal")) * speedX;

        if (Input.GetAxis("Vertical") >= 0) lookingUp = true;
        else lookingUp = false;

        if (Input.GetButton("Fire") && !isCharging && isGrounded) StartCoroutine(LoadShot());

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 10, LayerMask.GetMask("Raycast"));
        if (hit.collider != null) aboveEnemy = true;
        else aboveEnemy = false;
    }

    void FlipIfNeeded()
    {
        if (movement.x > 0)
        {
            if (!lookingRight)
            {
                sr.flipX = false;
                lookingRight = true;
            }
        }
        else if (movement.x < 0)
        {
            if (lookingRight)
            {
                sr.flipX = true;
                lookingRight = false;
            }
        }
    }

    void Shoot()
    {
        float dirX;
        float dirY;

        if (lookingUp) dirY = 0.0f;
        else dirY = -1.0f;

        if (dirY == -1.0f) dirX = 0.0f;
        else if (lookingRight) dirX = 1.0f;
        else dirX = -1.0f;

        Vector2 shotDirection = new Vector2(dirX, dirY).normalized;
        OnShoot?.Invoke(lastShotCharge, projectileSpeed, transform.position, shotDirection);

        if (dirY == -1.0f) SoundJump(shotDirection);
    }

    void SoundJump(Vector3 soundDir) //si no hay salto diagonal se puede sacar este parametro
    {
        rb.AddForce(-soundDir * (jumpBaseForce + (lastShotCharge * jumpForcePerChargeLvl)));
    }

    void CheckCollisionWithEnemy(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (!onHurtAnim)
            {
                if (aboveEnemy && rb.velocity.y < 0)
                {
                    rb.velocity = Vector2.zero;
                    SoundJump(-transform.up);
                    if (lastShotCharge > 0) lastShotCharge--;
                }
                else TakeDamage(collision.transform.position.x);
            }
        }
    }

    void TakeDamage(float collisionX)
    {
        if (invincible) return;

        float dirX = transform.position.x - collisionX;
        float dirY = Mathf.Abs(dirX);
        Vector2 dir = new Vector2(dirX, dirY).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * pushWhenHurtForce);
        onHurtAnim = true;

        Lives--;
        StartCoroutine(InvincibilityFrames());

        Debug.Log("player damaged, lives: " + Lives);
        if (Lives == 0) Debug.Log("player DIED");
    }

    IEnumerator LoadShot()
    {
        isCharging = true;
        float load = 0.0f;

        while (!Input.GetButtonUp("Fire"))
        {
            load += Time.deltaTime * chargingSpeed;
            yield return null;
        }

        if (load > 3.0f) load = 3.0f;
        lastShotCharge = (int)load;
        isCharging = false;
        Shoot();
    }

    IEnumerator InvincibilityFrames()
    {
        invincible = true;

        int repetitionNumber = 10;
        Color color = sr.color;

        for (int i = 0; i < repetitionNumber; i++)
        {
            if (i % 2 == 0)
            {
                color.a = 0.5f;
                sr.color = color;
            }
            else
            {
                color.a = 1.0f;
                sr.color = color;
            }

            yield return new WaitForSeconds(invincibilityDuration / repetitionNumber);
        }

        color.a = 1.0f;
        sr.color = color;

        invincible = false;
        exitedInvisibilityFrames = true;
    }
}