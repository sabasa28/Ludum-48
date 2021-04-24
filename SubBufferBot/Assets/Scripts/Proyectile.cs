using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField] 
    Transform daddy;
    float speed = 700.0f;
    bool shot;

    public void Fire(int chargeLvl, Vector3 direction)
    {
        transform.parent = null;
        transform.right = direction;
        transform.localScale *= chargeLvl+1;
        shot = true;
        rb.velocity = ((Vector2)transform.right) * Time.fixedDeltaTime * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            transform.parent = daddy;
            transform.localPosition= Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            shot = false;
            gameObject.SetActive(false);
        }
    }
}
