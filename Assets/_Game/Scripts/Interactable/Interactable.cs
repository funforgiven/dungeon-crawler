using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public virtual void OnInteract()
    {
        Debug.Log("Interacted");
    }
}