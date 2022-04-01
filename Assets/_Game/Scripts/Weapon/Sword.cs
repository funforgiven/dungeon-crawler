using System.Collections;
using UnityEngine;

public class Sword : Weapon
{
    [SerializeField] private float swingAngle = 90f;
    [SerializeField] private float swingDuration = 1f;

    private bool _canSwing = true;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public override void Attack()
    {
        if(_canSwing)
            StartCoroutine(Swing());
    }

    IEnumerator Swing()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        var _rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        
        _canSwing = false;
        
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        
        float timeElapsed = 0;
        while (timeElapsed < swingDuration)
        {
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, _rotZ - swingAngle/2),
                Quaternion.Euler(0f, 0f, _rotZ - 3*swingAngle/2), timeElapsed / swingDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        
        _canSwing = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var enemy = col.GetComponent<Enemy>();
        if(enemy) ApplyDamage(col.GetComponent<Enemy>());
    }
}
