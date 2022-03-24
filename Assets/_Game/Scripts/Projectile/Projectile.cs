using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private float damage;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layersToDamage;

    public GameObject owner;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        Explode(col.transform.position);
    }

    public IEnumerator Move(Vector3 startPosition, Vector3 endPosition)
    {
        float timeElapsed = 0;
        float timeToReach = Vector2.Distance(startPosition, endPosition)/speed;
        
        while (timeElapsed < timeToReach)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, timeElapsed / timeToReach);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        Explode(transform.position);
    }

    public void Move(Vector3 startPosition, Vector2 direction)
    {
        transform.position = startPosition;
        GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
    }

    void Explode(Vector3 position)
    {
        var overlapping = Physics2D.OverlapCircleAll(position, radius, layersToDamage);
        foreach(var col in overlapping)
        {
            ApplyDamage(col.GetComponent<IDamageable>());
        }
        
        Destroy(gameObject);
    }

    void ApplyDamage(IDamageable damageable)
    {
        damageable.TakeDamage(damage, owner);
    }
}
