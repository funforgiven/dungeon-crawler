using System.Collections.Generic;
using UnityEngine;


    
public class Event : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject enemyPrefab;

    public List<GameObject> Enemies;

    private bool _isInteractable = true;

    // Start is called before the first frame update
    public void OnInteract()
    {
        if (!_isInteractable) return;
        _isInteractable = false;
        var _enemy = Instantiate(enemyPrefab, transform.position - new Vector3(0, 7, 0), Quaternion.identity);
        Enemies.Add(_enemy);
        _enemy.GetComponent<Enemy>()._event = this;
    }

    public void OnEventEnd()
    {
        Destroy(gameObject);
    }
}
