using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpaqueBall : Ball
{
    public override void Select(Material highlightMaterialGlass, Material highlightMaterialOpaque, HashSet<Ball> selectedAlready)
    {
        _renderer.material = highlightMaterialOpaque;
    }
}