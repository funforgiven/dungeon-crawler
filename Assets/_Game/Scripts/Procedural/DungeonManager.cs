using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector2 = UnityEngine.Vector2;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private Vector3Int startPosition = new Vector3Int(0, 0, 0);
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    
    [Header("Binary Space Partition")]
    [SerializeField] private int dungeonWidth = 200;
    [SerializeField] private int dungeonHeight = 200;
    [SerializeField] private int roomMinWidth = 20;
    [SerializeField] private int roomMinHeight = 20;
    [SerializeField] private int roomMaxWidth = 35;
    [SerializeField] private int roomMaxHeight = 35;
    [SerializeField] private int maxRooms = 8;

    [Header("Random Walk")]
    [SerializeField] private int roomWalkLength = 150;
    [SerializeField] private int roomWalkIteration = 6;
    [SerializeField] private int offset = 1;

    private Dictionary<Vector2Int, HashSet<Vector2Int>> _rooms = new();

    // Start is called before the first frame update
    void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        var size = new Vector3Int(dungeonWidth, dungeonHeight, 0);
        var dungeon = new BoundsInt(startPosition, size);
        var roomBounds = DungeonUtility.BSP(dungeon, roomMinWidth, roomMinHeight, roomMaxWidth, roomMaxHeight, maxRooms);

        // All Room Tiles
        var roomTiles = CreateRooms(roomBounds); // This adds room tiles for each room into the _rooms dictionary.
        
        // All Corridor Tiles
        var corridorTiles = CreateCorridors(); // Uses _rooms which is created by CreateRooms() function called before this.
        
        // All Room and Corridor Tiles
        var floorTiles = new HashSet<Vector2Int>();
        floorTiles.UnionWith(roomTiles);
        floorTiles.UnionWith(corridorTiles);
        
        // All Wall Tiles
        var wallTiles = CreateWalls(floorTiles);
        
        Paint(floorTiles, floorTile);
        Paint(wallTiles, wallTile);
    }

    private HashSet<Vector2Int> CreateRooms(List<BoundsInt> rooms)
    {
        var result = new HashSet<Vector2Int>();
        
        foreach (var room in rooms)
        {
            var roomResult = new HashSet<Vector2Int>();
            var roomCenter = Vector2Int.RoundToInt(room.center);
            var position = roomCenter;
            
            for (int i = 0; i < roomWalkIteration; i++)
            {
                var walk = DungeonUtility.RandomWalk(position, roomWalkLength);

                foreach (var pos in walk)
                {
                    if (pos.x >= (room.xMin + offset) &&
                        pos.x <= (room.xMax - offset) &&
                        pos.y >= (room.yMin + offset) &&
                        pos.y <= (room.yMax - offset))
                        roomResult.Add(pos);
                }

                position = roomResult.ElementAt(Random.Range(0, roomResult.Count));
            }
            
            _rooms.Add(roomCenter, roomResult);
            result.UnionWith(roomResult);
        }

        return result;
    }
    private HashSet<Vector2Int> CreateCorridors()
    {
        var corridors = new HashSet<Vector2Int>();
        
        var centers = _rooms.Keys.ToHashSet();
        var currentCenter = centers.ElementAt(Random.Range(0, centers.Count));
        centers.Remove(currentCenter);
        
        while (centers.Count > 0)
        {
            Vector2Int closest = FindClosestCenter(currentCenter, centers);
            centers.Remove(closest);
            HashSet<Vector2Int> corridor = CreateCorridor(currentCenter, closest);
            corridors.UnionWith(corridor);
            
            currentCenter = closest;
        }

        return corridors;
    }

    private Vector2Int FindClosestCenter(Vector2Int currentCenter, HashSet<Vector2Int> centers)
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
    
    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentCenter, Vector2Int closest)
    {
        var corridor = new HashSet<Vector2Int>();
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
        }

        return corridor;
    }

    private HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floorTiles)
    {
        var walls = new HashSet<Vector2Int>();

        foreach (var pos in floorTiles)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    var nextPos = pos + new Vector2Int(i, k);
                    if (!floorTiles.Contains(nextPos))
                    {
                        walls.Add(nextPos);
                    }
                }
            }
        }

        return walls;
    }

    private void Paint(HashSet<Vector2Int> positions, TileBase tile)
    {
        foreach (var pos in positions.Select(position => tilemap.WorldToCell((Vector3Int)position)))
        {
            tilemap.SetTile(pos, tile);
        }
    }
}
