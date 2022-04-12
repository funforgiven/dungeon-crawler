using System.Collections;
using UnityEngine;

public class GoblinSword : EnemySword
{
    public override void Attack(string identifier = "", Sprite sprite = null)
    {
        if (_canStab)
        {
            _identifier = identifier;
            _spriteRenderer.sprite = sprite ? sprite : defaultSprite;
            StartCoroutine(Stab());
        }
    }

    IEnumerator Stab()
    {
        transform.position = owner.transform.position;

        Vector3 diff = owner._target.transform.position - transform.position;
        diff.Normalize();
        var rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ - 90f);

        _canStab = false;
        Enable();
        
        var pos = transform.position;
        float timeElapsed = 0;
        while (timeElapsed < _stabDuration)
        {
            transform.position = Vector3.Lerp(pos, pos + diff * stabLength, timeElapsed / _stabDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        var pos2 = transform.position;
        timeElapsed = 0;
        while (timeElapsed < _stabDuration)
        {
            transform.position = Vector3.Lerp(pos2, pos, timeElapsed / _stabDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        Disable();
        _canStab = true;
    }
}
