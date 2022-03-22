using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Imp : Enemy
{
    [SerializeField] private GameObject fireball;
    protected override void Attack()
    {
        CastFireball();
    }

    private void CastFireball()
    {
        var projectile = Instantiate(fireball, transform.position, quaternion.identity);
        var velocity = (_target.position - projectile.transform.position).normalized * projectile.GetComponent<Projectile>().speed;
        projectile.GetComponent<Rigidbody2D>().velocity = velocity;

    }
}
