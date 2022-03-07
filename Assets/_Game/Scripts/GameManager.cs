using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject player;

    [SerializeField] private Vector2 spawnPosition;
    // Start is called before the first frame update
    void Awake()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Instantiate(player, spawnPosition, Quaternion.identity);
    }
}
