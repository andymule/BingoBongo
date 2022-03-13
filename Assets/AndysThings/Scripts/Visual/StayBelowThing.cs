using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayBelowThing : MonoBehaviour
{
    [SerializeField] private float _lockAngleDifference = 45;
    [SerializeField] private Transform _thingToStayBelow;
    [SerializeField] private float _distanceBelowThing = .9f;

    void Update()
    {
        // lock the menu in place if we're looking at it, rotate freely otherwise
        var thingToThisVec = transform.position - _thingToStayBelow.position;
        if (!(Vector3.Angle(_thingToStayBelow.forward, thingToThisVec) < _lockAngleDifference))
        {
            transform.eulerAngles = new Vector3(0, _thingToStayBelow.eulerAngles.y, 0);
        }

        transform.position = _thingToStayBelow.position - new Vector3(0, _distanceBelowThing, 0);
    }
}