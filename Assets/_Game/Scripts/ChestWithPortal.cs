using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestWithPortal : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemToSpawn;
    [SerializeField] private GameObject portalToMove;
    [SerializeField] private Vector2 portalLocation;

    public void OnInteract()
    {
        Instantiate(itemToSpawn, transform.position, Quaternion.identity);
        portalToMove.transform.position = portalLocation;
        Destroy(gameObject);
    }
}
