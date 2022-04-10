using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
  public Slider slider;
public Vector3 offset;

    public void SetHealthBar(float Health, float MaxHealth){


      slider.value = Health;
      slider.maxValue = MaxHealth;


    }

    // Update is called once per frame
    void Update()
    {
      slider.transform.SetParent(transform);
    }
}
