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

    protected override void Awake()
    {
        base.Awake();
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
        if (dead) return;
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
        bool startedAnimating = false;
        while (timer < attackChargeDuration)
        {
            if (!startedAnimating && timer < attackChargeDuration / 3)
            {
                anim.SetTrigger("Attacking");
                startedAnimating = true;
            }

            timer += Time.deltaTime;
            yield return null;
        }
        chargingAttack = false;
        attack.OnCharged();

        StartCoroutine(CoolDown());
    }
}