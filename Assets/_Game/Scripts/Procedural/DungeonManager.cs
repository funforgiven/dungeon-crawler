using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private int roomCount = 5;
    [SerializeField] private int minWidth = 5;
    [SerializeField] private int maxWidth = 5;
    [SerializeField] private int minHeight = 5;
    [SerializeField] private int maxHeight = 5;
    
    [SerializeField] private TileBase tile;
    [SerializeField] private GameObject tilemap;
    [SerializeField] private GameObject grid;
    private List<Room> _rooms;
    private Room _lastRoom = null;
    
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < roomCount; i++)
        {
            int width = Random.Range(minWidth, maxWidth);
            int height = Random.Range(minHeight, maxHeight);
            TileBase[] tileArray = new TileBase[width*height];
            for (int index = 0; index < tileArray.Length; index++)
            {
                tileArray[index] = tile;
            }

            Room room = Instantiate(tilemap, new Vector3(i*maxWidth, 0, 0), Quaternion.identity).GetComponent<Room>();
            room.transform.SetParent(grid.transform);
            //_rooms.Add(room);
            
            if (_lastRoom != null)
            {
                _lastRoom.nextRoom = room;
            }

            _lastRoom = room;

            Tilemap map = room.GetComponent<Tilemap>();
            map.SetTilesBlock(new BoundsInt(-3, -3, 0, width, height, 1), tileArray);
        }
    }
}
