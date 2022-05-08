using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private int minWidth = 6;
    [SerializeField] private int minHeight = 6;
    [SerializeField] private int dungeonWidth = 6;
    [SerializeField] private int dungeonHeight = 6;
    [SerializeField] private int offset = 1;
    [SerializeField] private Vector3Int startPosition = new Vector3Int(0, 0, 0);

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tile;

    // Start is called before the first frame update
    void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        var size = new Vector3Int(dungeonWidth, dungeonHeight, 0);
        var dungeon = new BoundsInt(startPosition, size);
        var rooms = DungeonUtility.BSP(dungeon, minWidth, minHeight);

        Paint(CreateRooms(rooms));
        Paint(CreateCorridors(rooms));
    }
    
    private List<Vector2Int> CreateRooms(List<BoundsInt> rooms)
    {
        var tiles = new List<Vector2Int>();
        
        foreach (var room in rooms)
        {
            for (int i = offset; i < room.size.x - offset; i++)
            {
                for (int k = offset; k < room.size.y - offset; k++)
                {
                    var pos = (Vector2Int)room.min + new Vector2Int(i, k);
                    tiles.Add(pos);
                }
            }
        }

        return tiles;
    }
    
    private List<Vector2Int> CreateCorridors(List<BoundsInt> rooms)
    {
        var corridors = new List<Vector2Int>();
        
        var centers = rooms.Select(room => Vector2Int.RoundToInt(room.center)).ToList();
        var currentCenter = centers[Random.Range(0, centers.Count)];
        centers.Remove(currentCenter);

        while (centers.Count > 0)
        {
            Vector2Int closest = FindClosestCenter(currentCenter, centers);
            centers.Remove(closest);
            List<Vector2Int> corridor = CreateCorridor(currentCenter, closest);
            corridors.AddRange(corridor);
            
            currentCenter = closest;
        }

        return corridors;
    }

    private Vector2Int FindClosestCenter(Vector2Int currentCenter, List<Vector2Int> centers)
    {
        Vector2Int closest = Vector2Int.zero;
        var distance = float.MaxValue;
        foreach (var center in centers)
        {
            var dist = Vector2.Distance(currentCenter, center);
            if (dist < distance)
            {
                distance = dist;
                closest = center;
            }
        }

        return closest;
    }
    
    private List<Vector2Int> CreateCorridor(Vector2Int currentCenter, Vector2Int closest)
    {
        var corridor = new List<Vector2Int>();
        var pos = currentCenter;
        corridor.Add(pos);

        while (pos.y != closest.y)
        {
            if (closest.y > pos.y)
            {
                pos += Vector2Int.up;
            }
            else
            {
                pos += Vector2Int.down;
            }
            corridor.Add(pos);
            corridor.Add(pos + new Vector2Int(1, 0));
            corridor.Add(pos + new Vector2Int(-1, 0));
        }   
        
        while (pos.x != closest.x)
        {
            if (closest.x > pos.x)
            {
                pos += Vector2Int.right;
            }
            else
            {
                pos += Vector2Int.left;
            }
            corridor.Add(pos);
            corridor.Add(pos + new Vector2Int(0, 1));
            corridor.Add(pos + new Vector2Int(0, -1));
        }

        return corridor;
    }

    private void Paint(List<Vector2Int> positions)
    {
        foreach (var pos in positions.Select(position => tilemap.WorldToCell((Vector3Int)position)))
        {
            tilemap.SetTile(pos, tile);
        }
    }
}
