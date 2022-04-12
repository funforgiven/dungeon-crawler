using System.Collections;
using UnityEngine;

public class EnemySword : MonoBehaviour
{
    [Header("Swing")]
    [SerializeField] private float swingAngle = 90f;
    [SerializeField] public float defaultSwingDuration = 0.5f;
    internal float _swingDuration;
    private bool _canSwing = true;
    
    [Header("Stab")]
    [SerializeField] internal float stabLength = 90f;
    [SerializeField] public float defaultStabDuration = 0.5f;
    internal float _stabDuration;
    protected bool _canStab = true;

    [Header("Sprite")]
    [SerializeField] public Sprite defaultSprite;

    internal Enemy owner;
    protected string _identifier = "";
    protected SpriteRenderer _spriteRenderer;
    
    private void Start()
    {
        Disable();
        _swingDuration = defaultSwingDuration;
        _stabDuration = defaultStabDuration;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Attack(string identifier = "", Sprite sprite = null)
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
        Vector3 diff = owner._target.transform.position - transform.position;
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
        var hero = col.GetComponent<Hero>();

        if(hero) hero.TakeDamage(owner.swordDamage, owner.gameObject, DamageType.Physical);
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
