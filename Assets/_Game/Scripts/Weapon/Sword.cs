using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float swingAngle = 90f;
    [SerializeField] public float defaultSwingDuration = 0.5f;
    [SerializeField] public Sprite defaultSprite;
    
    internal Hero owner;
    
    internal float _swingDuration;
    private bool _canSwing = true;
    private string _identifier = "";
    private SpriteRenderer _spriteRenderer;
    
    private void Start()
    {
        Disable();
        _swingDuration = defaultSwingDuration;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack(string identifier = "", Sprite sprite = null)
    {
        if (_canSwing)
        {
            _identifier = identifier;
            _spriteRenderer.sprite = sprite ? sprite : defaultSprite;
            StartCoroutine(Swing());
        }
    }

    IEnumerator Swing()
    {
        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        var rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        
        _canSwing = false;
        
        Enable();
        
        float timeElapsed = 0;
        while (timeElapsed < _swingDuration)
        {
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, rotZ - swingAngle/2),
                Quaternion.Euler(0f, 0f, rotZ - 3*swingAngle/2), timeElapsed / _swingDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Disable();
        
        _canSwing = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var enemy = col.GetComponent<Enemy>();
        if(enemy) owner.GetComponent<Hero>().ApplyDamage(col.GetComponent<Enemy>(), _identifier);
    }
    
    public void Enable()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void Disable()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
