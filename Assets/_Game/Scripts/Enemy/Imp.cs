using System.Buffers;
using UnityEngine;

public class Imp : Enemy
{
    [SerializeField] private GameObject fireball;

    protected override void Update()
    {
        base.Update();
        
        _attackTimeElapsed += Time.deltaTime;
        
        if (_isAttacking && _attackTimeElapsed > 60 / attackRate)
        {
            _attackTimeElapsed = 0;

            var fireballSpawned = Instantiate(fireball, transform.position, Quaternion.identity).GetComponent<Fireball>();
            fireballSpawned.owner = this;
            fireballSpawned.target = _target;
        }
    }
}
