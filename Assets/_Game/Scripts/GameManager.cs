using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject followCamera;

    [SerializeField] private Vector2 spawnPosition;
    
    public static GameManager Instance { get; private set; }

    
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        
        StartGame();
    }

    private void StartGame()
    {
        SpawnPlayer();
        AttachCamera();
    }
    
    public void StartGame(GameObject hero)
    {
        player = hero;
        SpawnPlayer();
        AttachCamera();
    }
    
    private void SpawnPlayer()
    {
        player = Instantiate(player, spawnPosition, Quaternion.identity);
    }
    
    
    private void AttachCamera()
    {
        var cinemachine = followCamera.GetComponent<CinemachineVirtualCamera>();
        cinemachine.Follow = player.transform;
    }
}
