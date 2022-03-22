using UnityEngine;
using UnityEngine.AI;

public class Imp : Enemy
{
    [SerializeField] private GameObject fireball;
    [SerializeField] private float fireRate = 20f;

    private float _fireballTimeElapsed = 0;
    
    protected override void Update()
    {
        base.Update();
        
        _fireballTimeElapsed += Time.deltaTime;
        
        if (_isShooting && _fireballTimeElapsed > 60 / fireRate)
        {
            _fireballTimeElapsed = 0;
            
            var projectile = Instantiate(fireball).GetComponent<Projectile>();
            StartCoroutine(projectile.Move(transform.position, _target.position));
        }
    }
    
}