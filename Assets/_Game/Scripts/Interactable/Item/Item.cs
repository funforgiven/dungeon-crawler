using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string name;
    [SerializeField] private Texture2D icon;

    protected Hero owner;
    
    protected virtual void OnPickUp()
    {
        
    }

    public void OnInteract()
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        owner = hero;
        OnPickUp();
    }
}
