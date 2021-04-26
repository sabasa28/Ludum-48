using System;
using System.Collections;
using UnityEngine;

public class ChargedAttack : MonoBehaviour
{
    [HideInInspector] public bool charged = false;

    [SerializeField] float duration = 0.1f;

    public static event Action<float> OnPlayerDamaged;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (charged)
        {
            if (collision.CompareTag("Player")) OnPlayerDamaged?.Invoke(transform.position.x);

            StopCoroutine(AttackDurationTimer());
            gameObject.SetActive(false);
        }
    }

    public void OnCharged()
    {
        charged = true;

        StartCoroutine(AttackDurationTimer());
    }

    IEnumerator AttackDurationTimer()
    {
        yield return new WaitForSeconds(duration);

        charged = false;
        gameObject.SetActive(false);
    }
}