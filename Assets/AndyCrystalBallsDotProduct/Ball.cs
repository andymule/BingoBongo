using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    protected Material _startingMaterial;
    protected Renderer _renderer;
    protected BallManager _ballManager;

    public void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _startingMaterial = GetComponent<Renderer>().material;
        _ballManager = FindObjectOfType<BallManager>();
        _ballManager.RegisterNewBall(this);
    }

    // kind of gross overloaded select bc we only use some params for glass
    public virtual void Select(Material highlightMaterialGlass, Material highlightMaterialOpaque, HashSet<Ball> selectedAlready)
    {
        _renderer.material = highlightMaterialOpaque;
    }

    public virtual void DeSelect(HashSet<Ball> deselectedAlready)
    {
        _renderer.material = _startingMaterial;
    }

    private void OnDestroy()
    {
        _ballManager.DeRegisterNewBall(this);
    }
}