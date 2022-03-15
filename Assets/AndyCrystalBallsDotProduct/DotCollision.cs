using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DotCollision : MonoBehaviour
{
    [SerializeField] private Material highlightGlass;
    [SerializeField] private Material highlightOpaque;
    protected BallManager _ballManager;

    /// <summary>
    /// How big is the visual angle of a 1m sphere at 1m?
    /// This lets us quickly calculate if our dot product is hitting the sphere or not bc we know the sphere distance 
    /// </summary>
    private readonly float visualDegreeSizeOfA1MeterSphereAt1Meter = 30f;

    void Start()
    {
        _ballManager = FindObjectOfType<BallManager>();
    }

    void Update()
    {
        float closestBallHit = float.MaxValue;
        Ball closestBall = null;
        
        // TODO replace with more efficient deselecter, and only deselect on change
        foreach (var ball in _ballManager.AllBalls)
        {
            ball.DeSelect(new HashSet<Ball>());
        }

        foreach (var ball in _ballManager.AllBalls)
        {
            var thisBallDistance = Vector3.Distance(transform.position, ball.transform.position);

            // if we've hit a ball already, ignore balls that are farther away
            if (closestBall != null && thisBallDistance >= closestBallHit)
                continue;

            // TODO consider balls encompassing the camera
            var thisBallDir = (ball.transform.position - transform.position).normalized;
            var angleToThisBall = Mathf.Acos(Vector3.Dot(thisBallDir, transform.forward)) / Mathf.PI * 180;
            var thisBallVisualAngleSize = visualDegreeSizeOfA1MeterSphereAt1Meter * ball.transform.localScale.x / thisBallDistance;
            if (angleToThisBall <= thisBallVisualAngleSize)
            {
                // we know it's closer than previous ball bc we already checked above
                closestBall = ball;
                closestBallHit = thisBallDistance;
            }
        }
        if (closestBall != null)
        {
            closestBall.Select(highlightGlass, highlightOpaque, new HashSet<Ball>());
        }
    }
}