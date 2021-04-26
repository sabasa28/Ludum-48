using UnityEngine;

public class Projectile : MonoBehaviour
{
    Transform daddy;
    [SerializeField]
    Rigidbody2D rb = null;

    void Awake()
    {
        daddy = transform.parent;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Floor")) ReturnToPool();
    }

    protected virtual void ReturnToPool()
    {
        transform.parent = daddy;
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector2.one;

        gameObject.SetActive(false);
    }

    public virtual void Fire(float speed, Vector2 direction)
    {
        transform.parent = null;
        rb.velocity = direction * speed;
    }
}