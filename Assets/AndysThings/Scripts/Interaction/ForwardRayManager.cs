using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Shoots a ray out the middle of the camera and tries to hit tools
/// </summary>
public class ForwardRayManager : MonoBehaviour
{
    private Toolbelt _toolbelt;
    private Transform _cameraTransform;

    void Start()
    {
        _toolbelt = FindObjectOfType<Toolbelt>();
        _cameraTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, 20f, LayerMask.GetMask("Tool")))
        {
            _toolbelt.SelectTool(hit.transform.gameObject);
        }
    }
}