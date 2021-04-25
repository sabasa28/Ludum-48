using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    Vector2 movement;
    [SerializeField]
    float speedX = 0.0f;

    int lastShotPower;
    [SerializeField]
    float projectileSpeed = 2.0f;
    [SerializeField]
    float chargingSpeed = 1.0f;
    bool isCharging = false;

    bool lookingRight; //right==true  left==false
    bool lookingUp; //up==true  down==false     en verdad no es up, sino mirando hacia adelante

    float extraHorizontalPhysics;
    [SerializeField]
    float drag;

    [SerializeField]
    float jumpBaseForce;
    [SerializeField]
    float jumpForcePerChargeLvl;
    [SerializeField]
    float pushWhenHurtForce;

    [SerializeField]
    bool startsGrounded;
    bool isGrounded = false;

    bool aboveEnemy = false;

    bool onHurtAnim = false;
    bool frameSkipped = false;

    public static event Action<float, float, Vector2, Vector2> OnShoot;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        lookingRight = true;
        lookingUp = true;
        isGrounded = startsGrounded;
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
        OnShoot?.Invoke(lastShotPower + 1.0f, projectileSpeed, transform.position, shotDirection);

        if (dirY == -1.0f)
        {
            SoundJump(shotDirection);
        }

    }

    void SoundJump(Vector3 soundDir)//si no hay salto diagonal se puede sacar este parametro
    {
        rb.AddForce(-soundDir * (jumpBaseForce + (lastShotPower * jumpForcePerChargeLvl)));
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
        lastShotPower = (int)load;
        isCharging = false;
        Shoot();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !onHurtAnim)
        {
            if (aboveEnemy)
            {
                if (rb.velocity.y < 0)
                {
                    rb.velocity = Vector2.zero;
                    SoundJump(-transform.up);
                    if (lastShotPower > 0) lastShotPower--;
                }
            }
            else
            {
                float dirX = transform.position.x - collision.transform.position.x;
                float dirY = Mathf.Abs(dirX);
                Vector2 dir = new Vector2(dirX, dirY).normalized;
                rb.velocity = Vector2.zero;
                rb.AddForce(dir * pushWhenHurtForce);
                onHurtAnim = true;
            }
        }
    }
}