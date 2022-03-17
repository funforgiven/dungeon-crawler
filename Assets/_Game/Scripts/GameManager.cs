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
    // Start is called before the first frame update
    void Awake()
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
        var cinemachine = followCamera.GetComponent<CinemachineVirtualCamera>();
        cinemachine.Follow = player.transform;
    }
}
