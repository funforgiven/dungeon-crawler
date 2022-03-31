using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralTest : MonoBehaviour
{

    public TileBase tileA;
    public TileBase tileB;
    public BoundsInt area;
    
    
    // Start is called before the first frame update
    void Start()
    {
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y * area.size.z];
        for (int index = 0; index < tileArray.Length; index++)
        {
            tileArray[index] = index % 2 == 0 ? tileA : tileB;
        }
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.SetTilesBlock(area, tileArray);
    }
    
}
