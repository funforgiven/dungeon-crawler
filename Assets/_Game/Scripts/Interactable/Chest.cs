using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemToSpawn;

    public void OnInteract()
    {
        Instantiate(itemToSpawn, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
