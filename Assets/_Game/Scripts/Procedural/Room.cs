using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [HideInInspector] public Room nextRoom = null;
    public Teleporter teleporter;

    void Start()
    {
        teleporter = Instantiate(teleporter, transform.position + new Vector3(5, 5, 0), Quaternion.identity).GetComponent<Teleporter>();
        teleporter.room = this;
    }
}
