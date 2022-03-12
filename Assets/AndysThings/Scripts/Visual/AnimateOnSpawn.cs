using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

/// <summary>
/// Shoots things out of our torso to the desired destination
/// </summary>
public class AnimateOnSpawn : MonoBehaviour
{
    private Vector3 _startingPoint;
    private Quaternion _startingRotation;
    private Vector3 _destinationPoint;
    private readonly float TORSO_TO_HEAD_DIST = 0.5f; // how far below head do we start the object?
    private readonly float START_IN_FRONT_DIST = 0.25f; // how far below head do we start the object?
    private float _percentComplete = 0f; // tracks position on curve for object
    [SerializeField] private float animationTime = .5f;

    [SerializeField] private bool showImmediateWireframe = true; // show wireframe version of same object at destination ??
    [SerializeField] private GameObject wireObject; // the the wire version of the object here if enabled

    [SerializeField] private bool debugSpawnInFront;

    void Start()
    {
        _destinationPoint = this.transform.position; // anchor spawns in where it wants to end up
        
        // start at belt level and .25 meters forward
        _startingPoint = Camera.main.transform.position + new Vector3(0, -TORSO_TO_HEAD_DIST, 0) +
                         Camera.main.transform.forward * START_IN_FRONT_DIST;
        this.transform.position = _startingPoint; // we spawn it somewhere nice to show the animation of placing
        
        if (debugSpawnInFront)
            _destinationPoint = Camera.main.transform.forward * 2;

        wireObject.SetActive(showImmediateWireframe);
        wireObject.transform.position = _destinationPoint;
        wireObject.transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        _percentComplete += Time.deltaTime * 1 / animationTime;
        this.transform.position = Vector3.SlerpUnclamped(_startingPoint, _destinationPoint, _percentComplete);
        this.transform.rotation = Quaternion.SlerpUnclamped(_startingRotation, Quaternion.identity, _percentComplete);
        if (_percentComplete >= 1)
        {
            // remove this script and wireframe once we reach our destination
            Destroy(this);
            Destroy(wireObject);
        }
    }
}