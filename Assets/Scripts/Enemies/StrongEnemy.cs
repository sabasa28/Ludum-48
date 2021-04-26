using System.Collections;
using UnityEngine;

public class StrongEnemy : Enemy
{
    bool chargingAttack = false;

    [Header("Charged attack: ")]
    [SerializeField] float attackChargeDuration = 1.0f;
    float attackXOffset;

    [Header("Charged attack components: ")]
    [SerializeField] ChargedAttack attack;
    [SerializeField] SpriteRenderer attackSR;

    void Awake()
    {
        attackXOffset = Mathf.Abs(attack.transform.localPosition.x);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    void Update()
    {
        if (!chargingAttack) MoveTowardsPlayer();

        if (PlayerInRange() && attackReady && !chargingAttack) Attack();
    }

    protected override void Attack()
    {
        Vector2 attackPosition = transform.position;
        attackPosition.x += attackXOffset * GetPlayerDirection().x;
        attack.transform.position = attackPosition;
        attack.gameObject.SetActive(true);

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

        chargingAttack = false;

        attack.OnCharged();
        StartCoroutine(CoolDown());
    }
}