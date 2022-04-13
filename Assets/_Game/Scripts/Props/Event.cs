using System.Collections.Generic;
using System.Linq;
using UnityEngine;


    
public class Event : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>(); 
   
    [SerializeField] private int enemyCount;
    
    internal List<GameObject> enemies = new List<GameObject>();
    private List<Transform> enemySpawnPoints = new List<Transform>();
    
    private bool _isInteractable = true;

    public virtual void Start()
    {
        enemySpawnPoints = transform.GetComponentsInChildren<Transform>().ToList();
    }
    public virtual void OnInteract()
    {
        if (!_isInteractable) return;
        _isInteractable = false;

        for (int i = 0; i < enemyCount; i++)
        {
            if (enemySpawnPoints.Count == 0)
                return;
            
            var eFab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            var sFab = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Count)];
            var enemy = Instantiate(eFab, sFab.position, Quaternion.identity);
            
            enemies.Add(enemy);
            enemy.GetComponent<Enemy>()._event = this;
            
            enemySpawnPoints.Remove(sFab);
        }
    }

    public virtual void OnEventEnd()
    {
        Destroy(gameObject);
    }
}
