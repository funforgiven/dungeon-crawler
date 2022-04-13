using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    internal Camera _camera;

    [SerializeField] private LayerMask interactableLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Interact();
    }

    void Interact()
    {
        var hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, interactableLayerMask);
        if (hit.collider == null) return;
        
        var interactable = hit.collider.GetComponent<IInteractable>();
        if (interactable == null) return;
        
        
        interactable.OnInteract();
    }
}
