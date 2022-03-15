using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlassBall : Ball
{
    public List<Ball> GetAllBallsInsideMe(HashSet<Ball> alreadyConsidered)
    {
        var validBalls = new HashSet<Ball>( _ballManager.AllBalls ); // clone list
        validBalls.ExceptWith(alreadyConsidered);
        List<Ball> ballsInsideMe = new List<Ball>();
        var myPos = transform.position;
        var myRadius = transform.localScale.x / 2f; // assumes perfect sphere
        foreach (var ball in validBalls)
        {
            // the distance between the two points, but also considering both radii
            if (Vector3.Distance(myPos, ball.transform.position) - ball.transform.localScale.x / 2f < myRadius && ball != this)
            {
                ballsInsideMe.Add(ball);
            }
        }
        return ballsInsideMe;
    }

    /// <summary>
    /// Recursive selection on glass balls that selects all connected balls, and recurses into glass ones
    /// </summary>
    /// <param name="highlightMaterialGlass"></param>
    /// <param name="highlightMaterialOpaque"></param>
    /// <param name="selectedAlready"></param>
    public override void Select(Material highlightMaterialGlass, Material highlightMaterialOpaque, HashSet<Ball> selectedAlready)
    {
        _renderer.material = highlightMaterialGlass;
        selectedAlready.Add(this);
        GetAllBallsInsideMe(selectedAlready)
            .ForEach(ball => ball.Select(highlightMaterialGlass, highlightMaterialOpaque, selectedAlready)); // recurses through glass balls
    }

    public override void DeSelect(HashSet<Ball> deselectedAlready)
    {
        _renderer.material = _startingMaterial;
        deselectedAlready.Add(this);
        GetAllBallsInsideMe(deselectedAlready).ForEach(ball => ball.DeSelect(deselectedAlready)); // recurses through glass balls
    }
}