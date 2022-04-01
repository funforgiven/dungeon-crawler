using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whetstone : Item
{
    [SerializeField] private float critRateIncrease = 10f;
    
    protected override void OnPickUp()
    {
        owner.critRate += critRateIncrease;
        Destroy(gameObject);
    }
}
