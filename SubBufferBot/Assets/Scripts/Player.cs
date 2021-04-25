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
    bool startsGrounded;
    bool isGrounded = false;

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

        if (Input.GetAxisRaw("Vertical") >= 0) lookingUp = true;
        else lookingUp = false;

        if (Input.GetButton("Fire") && !isCharging && isGrounded) StartCoroutine(LoadShot());

    }

    void FixedUpdate()
    {
        FlipIfNeeded();
        //Debug.Log(extraXPhysics);
        if (!isCharging) 
            rb.velocity = new Vector2(movement.x + extraHorizontalPhysics, rb.velocity.y);
        else
            rb.velocity = new Vector2(extraHorizontalPhysics, rb.velocity.y);
        
        if (extraHorizontalPhysics!=0.0f) extraHorizontalPhysics *= drag;
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
        Debug.Log("lookingUp" + lookingUp);

        if (dirY == -1.0f && movement.x <= 0.1f && movement.x >=-0.1f) dirX = 0.0f;
        else if (lookingRight) dirX = 1.0f;
        else dirX = -1.0f;

        Vector2 shotDirection = new Vector2(dirX, dirY).normalized;
        OnShoot?.Invoke(lastShotPower + 1.0f, projectileSpeed, transform.position, shotDirection);

        if (dirY == -1.0f)
        {
            SoundJump(shotDirection);
        }

    }

    void SoundJump(Vector3 soundDir)
    {
        extraHorizontalPhysics = -soundDir.x * (jumpBaseForce + (lastShotPower * jumpForcePerChargeLvl)) / 25.0f;//harcodeado porque addforce usa valores mas altos que velocity
        soundDir = new Vector2(0.0f, soundDir.y).normalized;
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
            Debug.Log("GROUNDED");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
            Debug.Log("NO GROUNDED");
        }
    }
}