using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardRayManager : MonoBehaviour
{
    private Toolbelt _toolbelt;
    private Transform _cameraTransform;
    // [HideInInspector] public GameObject lastSpatialObject; // tracks last valid thing we looked at 
    
    void Start()
    {
        _toolbelt = FindObjectOfType<Toolbelt>();
        _cameraTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, 10f, LayerMask.GetMask("Tool")))
        {
            // Debug.DrawLine(_cameraTransform.position, hit.point);
            // print("HIT " + hit.transform.name);
            _toolbelt.SelectTool(hit.transform.gameObject);
        }
        else
        {
            _toolbelt.DeselectAll();
        }
    }
}