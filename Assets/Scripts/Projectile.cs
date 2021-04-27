using UnityEngine;

public class Projectile : MonoBehaviour
{
    Transform daddy;
    [SerializeField]
    Rigidbody2D rb = null;
    SpriteRenderer sr;

    void Awake()
    {
        daddy = transform.parent;
        sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Edge Collider")) ReturnToPool();
    }

    public virtual void ReturnToPool()
    {
        transform.parent = daddy;
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector2.one;
        sr.flipX = false;

        gameObject.SetActive(false);
    }

    public virtual void Fire(float speed, Vector2 direction)
    {
        transform.parent = null;
        rb.velocity = direction * speed;
        if (direction.x < 0) sr.flipX = true;
        if (direction.y < 0) transform.Rotate(0,0,-90);
    }
}