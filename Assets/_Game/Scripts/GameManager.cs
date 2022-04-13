using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] public GameObject player;
    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private GameObject respawnSword; 
    
    private GameObject oldPlayer;
    private GameObject _player;


    public static GameManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject);
        }
        else 
        {
            Instance = this;
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);

            oldPlayer = player;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Tutorial map1")
            StartGame();
        else
            StartDeathScene();
    }
    
    private void StartGame()
    {
        SpawnPlayer();
        AttachCamera();
    }

    private void StartDeathScene()
    {
        SpawnOldPlayer();
        
        var enemySpawnPosition = new Vector2(spawnPosition.x + 7.5f, spawnPosition.y);
        var enemy = Instantiate(player.GetComponent<Hero>().enemy, enemySpawnPosition, Quaternion.identity).GetComponent<Enemy>();
        enemy.GetComponent<Animator>().SetBool("Move", true);
        enemy.enabled = false;

        _player.GetComponent<Hero>().enabled = false;
        _player.transform.rotation = Quaternion.Euler(0, 0, 90);
        Destroy(_player.GetComponentInChildren<Canvas>().gameObject);
        AttachCamera();
        
        var respawnSwordSpawnPosition = new Vector2(spawnPosition.x, spawnPosition.y - 2);
        Instantiate(respawnSword, respawnSwordSpawnPosition, Quaternion.Euler(0, 0, -135));

        StartCoroutine(Move(enemy.transform, enemy.transform.position, respawnSwordSpawnPosition));
    }

    private IEnumerator Move(Transform t, Vector3 start, Vector3 end)
    {
        float timeElapsed = 0;
        while (timeElapsed < 4f)
        {
            t.position = Vector3.Lerp(start, end, timeElapsed / 4f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("Tutorial map1");
    }
    private void SpawnPlayer()
    {
        _player = Instantiate(player, spawnPosition, Quaternion.identity);
    }

    private void SpawnOldPlayer()
    {
        _player = Instantiate(oldPlayer, spawnPosition, Quaternion.identity);
        oldPlayer = player;
    }
    
    private void AttachCamera()
    {
        var cinemachine = Camera.main.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachine.Follow = _player.transform;
        
        GetComponent<CursorController>()._camera = Camera.main;
    }
    
}
