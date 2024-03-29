using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class DungeonManager : MonoBehaviour
{
    [Header("Dungeon")]
    [SerializeField] private Vector3Int startPosition = new Vector3Int(0, 0, 0);
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase tile;
    [SerializeField] private TileBase dummyTile;
    [SerializeField] private NavMeshSurface2d navmesh;
    private Dictionary<BoundsInt, HashSet<Vector2Int>> _rooms = new();
    private HashSet<Vector2Int> _allTiles = new();
    
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
    
    [Header("Hero")]
    [SerializeField] private GameObject hero;
    [SerializeField] private GameObject respawnSword;
    private GameObject _player;

    [Header("Enemies")] 
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private int minEnemyCount;
    [SerializeField] private int maxEnemyCount;
    
    //[Header("Props")]
    
    
    public static DungeonManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            hero = Instance.hero;
            Destroy(Instance);
        }
        
        Instance = this;
        DontDestroyOnLoad(this);
    }
    
    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        GenerateDungeon();
        SpawnHero();
        SpawnEnemies();
    }

    private void SpawnHero()
    {
        _player = Instantiate(hero, _rooms.ElementAt(0).Key.center, Quaternion.identity);
        
        var cinemachine = Camera.main.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachine.Follow = _player.transform;
        
        GetComponent<CursorController>()._camera = Camera.main;
    }

    private void SpawnEnemies()
    {
        for (int i = 1; i < _rooms.Count - 1; i++)
        {
            var room = _rooms.ElementAt(i).Value;
            var enemyCount = Random.Range(minEnemyCount, maxEnemyCount + 1);

            for (int k = 0; k < enemyCount; k++)
            {
                var pos = room.ElementAt(Random.Range(0, room.Count));
                Instantiate(enemies[Random.Range(0, enemies.Count)], (Vector2)pos, Quaternion.identity);
                room.Remove(pos);
            }
        }
    }

    public void OnDeath(GameObject newHero)
    {
        var enemySpawnPosition = new Vector2(5000f + 7.5f, 5000f);
        var enemy = Instantiate(newHero.GetComponent<Hero>().enemy, enemySpawnPosition, Quaternion.identity).GetComponent<Enemy>();
        enemy.GetComponent<Animator>().SetBool("Move", true);
        enemy.enabled = false;

        Destroy(_player);
        _player = Instantiate(hero, new Vector2(5000f, 5000f), Quaternion.identity);
        _player.GetComponent<Hero>().enabled = false;
        _player.transform.rotation = Quaternion.Euler(0, 0, 90);
        Destroy(_player.GetComponentInChildren<Canvas>().gameObject);
        var cinemachine = Camera.main.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemachine.Follow = _player.transform;

        var respawnSwordSpawnPosition = new Vector2(5000f, 5000f - 2f);
        var _respawnSword = Instantiate(respawnSword, respawnSwordSpawnPosition, Quaternion.Euler(0, 0, -135));

        hero = newHero;
        StartCoroutine(FinishDeath(enemy.transform, enemy.transform.position, respawnSwordSpawnPosition));
    }
    
    private IEnumerator FinishDeath(Transform t, Vector3 start, Vector3 end)
    {
        float timeElapsed = 0;
        while (timeElapsed < 4f)
        {
            t.position = Vector3.Lerp(start, end, timeElapsed / 4f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("Procedural");
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
        
        // All Tiles
        _allTiles.UnionWith(floorTiles);
        _allTiles.UnionWith(wallTiles);

        Paint(_allTiles, tile, groundTilemap);
        Paint(wallTiles, dummyTile, wallTilemap);
        
        navmesh.BuildNavMesh();
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
            
            _rooms.Add(room, roomResult);
            result.UnionWith(roomResult);
        }

        return result;
    }
    private HashSet<Vector2Int> CreateCorridors()
    {
        var corridors = new HashSet<Vector2Int>();

        var centers = _rooms.Keys.Select(room => Vector2Int.RoundToInt(room.center)).ToHashSet();
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

    private void Paint(HashSet<Vector2Int> positions, TileBase tile, Tilemap tilemap)
    {
        foreach (var pos in positions.Select(position => tilemap.WorldToCell((Vector3Int)position)))
        {
            tilemap.SetTile(pos, tile);
        }
    }
}
