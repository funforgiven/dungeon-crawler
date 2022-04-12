using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    protected override void Start()
    {
        base.Start();

        swordPrefab = Instantiate(swordPrefab, transform.position, Quaternion.identity);
        swordPrefab.transform.SetParent(transform);

        sword = swordPrefab.GetComponent<EnemySword>();
        sword.owner = this;
    }

    protected override void Update()
    {
        base.Update();

        _attackTimeElapsed += Time.deltaTime;

        if (_isAttacking && _attackTimeElapsed > 60 / attackRate)
        {
            _attackTimeElapsed = 0;

            sword.Attack();
        }
    }
}
