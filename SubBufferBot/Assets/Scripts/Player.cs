using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;

    Vector2 movement;
    [SerializeField]
    float speedX = 0.0f;

    [SerializeField]
    ProyectileManager proyManager;
    int lastShotPower;
    bool isShotInCD;
    [SerializeField]
    float shootingCD;
    [SerializeField]
    float charginSpeed;

    bool lookingRight;//right==true  left==false
    bool lookingUp;//up==true  down==false     en verdad no es up, sino mirando hacia adelante

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        lookingRight = true;
        lookingUp = true;
    }

    void Update()
    {
        movement.x = (Input.GetAxis("Horizontal")) * speedX;
        if (Input.GetAxisRaw("Vertical") >= 0) lookingUp = true;
        else lookingUp = false;
        if (Input.GetButtonDown("Fire") && !isShotInCD) StartCoroutine(LoadShot());
    }

    private void FixedUpdate()
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
    IEnumerator LoadShot()
    {
        isShotInCD = true;
        float load = 0.0f;
        while (!Input.GetButtonUp("Fire"))
        {
            load += Time.deltaTime * charginSpeed;
            yield return null;
        }
        if (load > 3.0f) load = 3.0f;
        lastShotPower = (int)load;
        Shoot();
    }

    void Shoot()
    {
        float dirX;
        float dirY;
        if (lookingUp) dirY = 0.0f;
        else dirY = -1.0f;
        if (dirY == -1.0f && movement.x == 0.0f)
            dirX = 0.0f;
        else if (lookingRight) dirX = 1.0f;
        else dirX = -1.0f;
        Vector3 shotDirection = new Vector3(dirX, dirY, 0).normalized;
        Debug.Log(movement);
        Debug.Log(shotDirection);
        proyManager.ReleaseProyectile(lastShotPower, shotDirection);
        StartCoroutine(CountCooldown());
    }

    IEnumerator CountCooldown()
    {
        yield return new WaitForSeconds(shootingCD);
        isShotInCD = false;
    }
}
