using UnityEngine;

public class Portal : MonoBehaviour
{
   [SerializeField] private Vector2 teleportPosition;
   private void OnTriggerEnter2D(Collider2D other)
   {
      other.transform.position = teleportPosition;
   }
}
