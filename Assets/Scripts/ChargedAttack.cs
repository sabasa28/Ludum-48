using System;
using System.Collections;
using UnityEngine;

public class ChargedAttack : MonoBehaviour
{
    [HideInInspector] public bool charged = false;

    [SerializeField] float duration = 0.1f;

    public static event Action<float, Enemy.Enemies> OnPlayerDamaged;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (charged)
        {
            if (collision.CompareTag("Player")) OnPlayerDamaged?.Invoke(transform.position.x, Enemy.Enemies.Strong);

            StopCoroutine(AttackDurationTimer());
            gameObject.SetActive(false);
        }
    }

    public void OnCharged()
    {
        charged = true;

        if (gameObject.activeInHierarchy) StartCoroutine(AttackDurationTimer());
    }

    IEnumerator AttackDurationTimer()
    {
        yield return new WaitForSeconds(duration);

        charged = false;
        gameObject.SetActive(false);
    }
}