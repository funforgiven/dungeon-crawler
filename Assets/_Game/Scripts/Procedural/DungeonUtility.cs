using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class DungeonUtility
{
    public static List<BoundsInt> BSP(BoundsInt area, int minWidth, int minHeight)
    {
        List<BoundsInt> rooms = new List<BoundsInt>();
        
        Queue<BoundsInt> roomQueue= new Queue<BoundsInt>();
        roomQueue.Enqueue(area);

        while (roomQueue.Count > 0)
        {
            var room = roomQueue.Dequeue();
            if(room.size.x >= minWidth*2 && room.size.y >= minHeight*2)
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
}
