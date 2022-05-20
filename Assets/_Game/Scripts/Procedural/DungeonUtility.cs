using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class DungeonUtility
{
    public static List<BoundsInt> BSP(BoundsInt area, int minWidth, int minHeight, int maxWidth, int maxHeight, int maxRooms)
    {
        List<BoundsInt> rooms = new List<BoundsInt>();
        
        Queue<BoundsInt> roomQueue= new Queue<BoundsInt>();
        roomQueue.Enqueue(area);

        while (roomQueue.Count > 0)
        {
            if (rooms.Count >= maxRooms)
                return rooms;
            
            var room = roomQueue.Dequeue();
            if(room.size.x >= maxWidth && room.size.y >= maxHeight)
            {
                if (Random.value <= 0.5f)
                {
                    SplitHorizontally(minHeight, room, roomQueue);
                }
                else
                {
                    SplitVertically(minWidth, room, roomQueue);
                }
            }
            else if (room.size.x >= minWidth * 2)
            {
                SplitVertically(minWidth, room, roomQueue);
            }
            else if (room.size.y >= minHeight * 2)
            {
                SplitHorizontally(minHeight, room, roomQueue);
            }
            else
            {
                if(room.size.x >= minWidth && room.size.y > minHeight)
                    rooms.Add(room);
            }
        }

        return rooms;
    }

    private static void SplitVertically(int minWidth, BoundsInt room, Queue<BoundsInt> roomQueue)
    {
        var x = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(x, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + x, room.min.y, room.min.z), new Vector3Int(room.size.x - x, room.size.y, room.size.z));
        
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }
    
    private static void SplitHorizontally(int minHeight, BoundsInt room, Queue<BoundsInt> roomQueue)
    {
        var y = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + y, room.min.z), new Vector3Int(room.size.x, room.size.y - y, room.size.z));
        
        roomQueue.Enqueue(room1);
        roomQueue.Enqueue(room2);
    }

    public static HashSet<Vector2Int> RandomWalk(Vector2Int startPos, int length)
    {
        var result = new HashSet<Vector2Int>();
        
        result.Add(startPos);
        var prevPos = startPos;

        for (int i = 0; i < length; i++)
        {
            var newPos = prevPos + directions[Random.Range(0, directions.Count)];
            result.Add(newPos);
            prevPos = newPos;
        }

        return result;
    }
    
    private static List<Vector2Int> directions = new List<Vector2Int>()
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };
}
