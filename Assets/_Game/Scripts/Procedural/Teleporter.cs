using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Room room;
    private void OnTriggerEnter2D(Collider2D col)
    {
        col.transform.position = room.nextRoom.transform.position;
    }
}
