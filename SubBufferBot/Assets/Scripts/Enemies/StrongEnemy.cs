using System.Collections;
using UnityEngine;

public class StrongEnemy : Enemy
{
    bool chargingAttack = false;

    [SerializeField] float attackChargeDuration = 1.0f;

    ChargedAttack attack = null;
    SpriteRenderer attackSR = null;
    [SerializeField] GameObject chargedAttackPrefab = null;

    void Update()
    {
        if (!chargingAttack) MoveTowardsPlayer();

        if (PlayerInRange() && attackReady && !chargingAttack) Attack();
    }

    protected override void Attack()
    {
        Vector2 attackPosition = transform.position;
        attackPosition += GetPlayerDirection();
        attack = Instantiate(chargedAttackPrefab, attackPosition, Quaternion.identity, transform).GetComponent<ChargedAttack>();
        attackSR = attack.GetComponent<SpriteRenderer>();

        StartCoroutine(ChargeAttack());
    }

    IEnumerator ChargeAttack()
    {
        float timer = 0.0f;

        chargingAttack = true;

        Color attackColor = attackSR.color;
        attackColor.a = 0.0f;
        attackSR.color = attackColor;

        while (timer < attackChargeDuration)
        {
            attackColor.a += Time.deltaTime / attackChargeDuration;
            attackSR.color = attackColor;

            timer += Time.deltaTime;
            yield return null;
        }

        attack.charged = true;

        chargingAttack = false;
        attack = null;
        attackSR = null;

        StartCoroutine(CoolDown());
    }
}