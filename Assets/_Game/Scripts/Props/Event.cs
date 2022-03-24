using System.Collections.Generic;
using UnityEngine;


    
public class Event : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private int enemyCount;
    [HideInInspector] public List<GameObject> enemies;

    private bool _isInteractable = true;

    // Start is called before the first frame update
    public void OnInteract()
    {
        if (!_isInteractable) return;
        _isInteractable = false;

        for (int i = 0; i < enemyCount; i++)
        {
            var _enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position - new Vector3(0, 7, 0), Quaternion.identity);
            enemies.Add(_enemy);
            _enemy.GetComponent<Enemy>()._event = this;
        }

    }

    public void OnEventEnd()
    {
        Destroy(gameObject);
    }
}
