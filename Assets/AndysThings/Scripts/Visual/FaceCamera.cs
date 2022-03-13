using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform _cameraTransform;
    [SerializeField] private float nudgeAwayThisMuch = 0f; // push away from user some amount, if desired

    void Start()
    {
        _cameraTransform = Camera.main.transform; 
    } 

    void Update()
    {
        transform.LookAt(_cameraTransform);
        
        if (nudgeAwayThisMuch != 0)
        {
            var nudgeDirection = this.transform.parent.position - _cameraTransform.position;
            this.transform.localPosition = nudgeDirection.normalized * nudgeAwayThisMuch;
        }
    }
}