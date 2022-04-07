using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string name;
    [SerializeField] public Sprite icon;
    [SerializeField] public GameObject image;



    protected Hero owner;

    protected virtual void OnPickUp()
    {

    }

    public void OnInteract()
    {
        Hero hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        owner = hero;
        OnPickUp();

        var panel = GameObject.FindWithTag("Panel");

        var notimage = Instantiate(image,panel.transform,true);

        notimage.GetComponent<Image>().sprite = icon;
    }
}
