using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] public GameObject player;

    [SerializeField] private Vector2 spawnPosition;
    
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
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartGame();
    }
    
    private void StartGame()
    {
        SpawnPlayer();
        AttachCamera();
    }

    private void SpawnPlayer()
    {
        player = Instantiate(player, spawnPosition, Quaternion.identity);
    }
    
    
    private void AttachCamera()
    {
        var cinemachine = Camera.main.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachine.Follow = player.transform;
    }
}
