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

    bool coolingDown;
    int lastShotPower;
    [SerializeField]
    float projectileSpeed = 2.0f;
    [SerializeField]
    float shootingCD = 1.0f;
    [SerializeField]
    float chargingSpeed = 1.0f;

    bool lookingRight; //right==true  left==false
    bool lookingUp; //up==true  down==false     en verdad no es up, sino mirando hacia adelante

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
    }

    void Update()
    {
        movement.x = (Input.GetAxis("Horizontal")) * speedX;

        if (Input.GetAxisRaw("Vertical") >= 0) lookingUp = true;
        else lookingUp = false;

        if (Input.GetButtonDown("Fire") && !coolingDown) StartCoroutine(LoadShot());
    }

    void FixedUpdate()
    {
        FlipIfNeeded();
        rb.velocity = movement * Time.fixedDeltaTime;
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

        if (dirY == -1.0f && movement.x == 0.0f) dirX = 0.0f;
        else if (lookingRight) dirX = 1.0f;
        else dirX = -1.0f;

        Vector2 shotDirection = new Vector2(dirX, dirY).normalized;
        OnShoot?.Invoke(lastShotPower + 1.0f, projectileSpeed, transform.position, shotDirection);

        StartCoroutine(CountCoolDown());
    }

    IEnumerator LoadShot()
    {
        coolingDown = true;
        float load = 0.0f;

        while (!Input.GetButtonUp("Fire"))
        {
            load += Time.deltaTime * chargingSpeed;
            yield return null;
        }

        if (load > 3.0f) load = 3.0f;
        lastShotPower = (int)load;

        Shoot();
    }

    IEnumerator CountCoolDown()
    {
        yield return new WaitForSeconds(shootingCD);
        coolingDown = false;
    }
}