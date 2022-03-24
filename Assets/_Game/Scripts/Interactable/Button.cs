using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
   public void OnInteract()
   {
      Debug.Log("Button");
   }
}
