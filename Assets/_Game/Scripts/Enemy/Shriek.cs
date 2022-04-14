using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shriek : Enemy
{
    [Header("AoE Attack")]
    [SerializeField] private float aoeDamage = 15f;
    [SerializeField] private float aoeDamageRadius = 3f;
    [SerializeField] private LayerMask aoeDamageLayer;
    [SerializeField] private float chaseRangeOffset;
    [SerializeField] private GameObject AOEAttack;

    [Header("Dodge")]
    [SerializeField] private float dodgeDuration = 5f;
    [SerializeField] private int dodgeCharge = 2;
    private bool _dodgeActive;
    private float _dodgeCurrentDuration = 0f;
    private int _dodgeCurrentCharge = 0;
    private bool _firstDamage = true;



    protected override void Start()
    {
        base.Start();

        attackRange = aoeDamageRadius;
        chaseRange = attackRange - chaseRangeOffset;
    }

    protected override void Update()
    {
        base.Update();

        _attackTimeElapsed += Time.deltaTime;

        if (_isAttacking && _attackTimeElapsed > 60 / attackRate)
        {

            _attackTimeElapsed = 0;

            var overlap = Physics2D.OverlapCircleAll(transform.position, aoeDamageRadius, aoeDamageLayer);

            foreach (var col in overlap)
            {
                var damageable = col.GetComponent<IDamageable>();
                if(damageable != null) damageable.TakeDamage(aoeDamage, gameObject, DamageType.Magical);
            }
            Instantiate(AOEAttack, transform.position, Quaternion.identity);
        }

        if (_dodgeActive)
        {
            _dodgeCurrentDuration += Time.deltaTime;
            if (_dodgeCurrentDuration > dodgeDuration)
            {
                _dodgeActive = false;
            }
        }

    }

    public override void TakeDamage(float damage, GameObject damager, DamageType damageType)
    {
        if (_dodgeActive)
        {
            _dodgeCurrentCharge -= 1;

            if (_dodgeCurrentCharge == 0)
                _dodgeActive = false;

            return;
        }

        if (_firstDamage)
        {
            _dodgeActive = true;
            _dodgeCurrentDuration = 0;
            _dodgeCurrentCharge = dodgeCharge;

            _firstDamage = false;
        }

        base.TakeDamage(damage, damager, damageType);
    }
}
