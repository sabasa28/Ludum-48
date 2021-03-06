using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    AudioManager audioManager;
    [HideInInspector] public SpriteRenderer sr;

    [Header("Health: ")]
    public int lives;

    [SerializeField] float invincibilityDuration = 3.0f;
    bool exitedInvisibilityFrames = false;
    [HideInInspector] public bool invincible = false;

    [SerializeField] Lives livesDisplayer;

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

    [SerializeField] float bounceRaycastLength;

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

    [SerializeField] ChargeBar chargeBar;

    float colliderWidth;

    public static event Action<int, float, Vector2, Vector2> OnShoot;
    public static event Action OnDeath;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioManager = AudioManager.Get();
    }

    void OnEnable()
    {
        EnemyProjectile.OnPlayerDamaged += TakeDamage;
        ChargedAttack.OnPlayerDamaged += TakeDamage;
    }

    void Start()
    {
        lookingRight = true;
        lookingUp = true;
        isGrounded = startsGrounded;
        colliderWidth = GetComponent<BoxCollider2D>().size.x;
    }

    void FixedUpdate()
    {
        FlipIfNeeded();

        if (onHurtAnim) 
        {
            if (frameSkipped)
            {
                if (rb.velocity == Vector2.zero)
                {
                    onHurtAnim = false;
                    anim.SetBool("Hurt", false);
                }
                frameSkipped = false;
            }
            else
                frameSkipped = true;
        }
        else if (!isCharging)
        {
            rb.velocity = new Vector2(movement.x + extraHorizontalPhysics, rb.velocity.y);
            anim.SetFloat("WalkSpeed", Mathf.Abs(movement.x / speedX));
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

        if (collision.CompareTag("Void")) OnFallenIntoVoid();
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
        anim.SetBool("OnAir", false);
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;

            chargeBar.ResetCharge();
            audioManager.PlaySound(AudioManager.Sounds.MetalImpact);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform")) isGrounded = false;
    }

    void Update()
    {
        movement = Vector2.zero;
        movement.x = (Input.GetAxis("Horizontal")) * speedX;

        if (Input.GetAxis("Vertical") >= 0)
        {
            lookingUp = true;
            anim.SetBool("LookingDown", false);
        }
        else
        {
            lookingUp = false;
            anim.SetBool("LookingDown", true);
        }

        if (Input.GetButton("Fire") && !isCharging && isGrounded) StartCoroutine(LoadShot());

        RaycastHit2D hit1 = Physics2D.Raycast(new Vector2 (transform.position.x + colliderWidth/2, transform.position.y), -transform.up, bounceRaycastLength, LayerMask.GetMask("Raycast"));
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2 (transform.position.x - colliderWidth/2, transform.position.y), -transform.up, bounceRaycastLength, LayerMask.GetMask("Raycast"));
        
        if (hit1.collider != null || hit2.collider != null) aboveEnemy = true;
        else aboveEnemy = false;
    }

    void OnDisable()
    {
        EnemyProjectile.OnPlayerDamaged -= TakeDamage;
        ChargedAttack.OnPlayerDamaged -= TakeDamage;
    }

    void FlipIfNeeded()
    {
        if (onHurtAnim) return;
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

        audioManager.PlaySoundWave(lastShotCharge);

        if (dirY == -1.0f) SoundJump(shotDirection);
        else
        {
            lastShotCharge = 0;
            chargeBar.ResetCharge();
        }
    }

    void SoundJump(Vector3 soundDir) //si no hay salto diagonal se puede sacar este parametro
    {
        rb.AddForce(-soundDir * (jumpBaseForce + (lastShotCharge * jumpForcePerChargeLvl)));
        anim.SetBool("OnAir", true);
    }

    void CheckCollisionWithEnemy(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (!onHurtAnim)
            {
                if (aboveEnemy)
                {
                    if (rb.velocity.y < 0)
                    {
                        rb.velocity = Vector2.zero;
                        SoundJump(-transform.up);
                        collision.GetComponent<Enemy>().Squished(lastShotCharge);
                        if (lastShotCharge > 0)
                        {
                            lastShotCharge--;
                            chargeBar.SetCharge(lastShotCharge + 1);
                        }
                        else chargeBar.ResetCharge();
                    }
                }
                else
                {
                    Enemy enemy = collision.GetComponent<Enemy>();
                    if (!enemy.dead) TakeDamage(collision.transform.position.x, enemy.enemyKind);
                }
            }
        }
    }

    void OnFallenIntoVoid()
    {
        transform.position = Vector2.zero;

        TakeDamage(1);
    }

    void TakeDamage(int damage)
    {
        if (invincible) return;

        lives -= damage;
        livesDisplayer.UpdateLives(lives);
        StartCoroutine(InvincibilityFrames());

        if (lives <= 0) OnDeath?.Invoke();
    }

    void TakeDamage(float collisionX, Enemy.Enemies enemyKind)
    {
        if (invincible) return;

        float dirX = transform.position.x - collisionX;
        float dirY = Mathf.Abs(dirX);
        Vector2 dir = new Vector2(dirX, dirY).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * pushWhenHurtForce);
        onHurtAnim = true;
        anim.SetBool("Hurt", true);

        if (dirX < 0.0f)
        {
            sr.flipX = false;
            lookingRight = true;
        }
        else
        {
            sr.flipX = true;
            lookingRight = false;
        }

        int damage = enemyKind == Enemy.Enemies.Strong ? 2 : 1;
        TakeDamage(damage);
    }

    public void SetLives(int newLives)
    {
        lives = newLives;
        livesDisplayer.UpdateLives(lives);
    }

    IEnumerator LoadShot()
    {
        isCharging = true;
        anim.SetBool("Charging", true);
        float load = 0.0f;
        int nextLoad = 0;

        while (!Input.GetButtonUp("Fire"))
        {
            load += Time.deltaTime * chargingSpeed;
            if (nextLoad <= 3 && load >= nextLoad)
            {
                chargeBar.SetCharge((int)load + 1);
                audioManager.PlaySound(AudioManager.Sounds.Charge);

                nextLoad++;
            }

            yield return null;
        }

        if (load > 3.0f) load = 3.0f;
        lastShotCharge = (int)load;
        isCharging = false;
        anim.SetBool("Charging", false);
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