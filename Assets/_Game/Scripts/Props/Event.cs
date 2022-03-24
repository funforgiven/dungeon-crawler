using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    
public class Event : MonoBehaviour
{
    private bool CheckEventDead = false;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update(){
    
        if (CheckEventDead = true) {
            Destroy(gameObject);
        }
    }
}
