using UnityEngine;

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
            
            var projectile = Instantiate(fireball, transform.position, Quaternion.identity).GetComponent<Fireball>();
            projectile.owner = gameObject;
            projectile.target = _target;
        }
    }
    
}
