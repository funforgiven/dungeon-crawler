using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Goblin : Enemy
{
    [Header("Sprint")]
    [SerializeField] private float sprintSpeed = 1.5f;

    protected override void Start()
    {
        base.Start();

        swordPrefab = Instantiate(swordPrefab, transform.position, Quaternion.identity);
        swordPrefab.transform.SetParent(transform);

        sword = swordPrefab.GetComponent<GoblinSword>();
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

    protected override IEnumerator FirstEncounter()
    {
        StartCoroutine(base.FirstEncounter());
        
        _agent.speed = defaultWalkSpeed * sprintSpeed;

        while (!_isAttacking)
            yield return null;

        _agent.speed = defaultWalkSpeed;
    }
}
