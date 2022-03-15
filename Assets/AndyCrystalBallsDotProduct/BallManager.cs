using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public HashSet<Ball> AllBalls = new HashSet<Ball>();

    public void RegisterNewBall(Ball ball)
    {
        AllBalls.Add(ball);
    }

    public void DeRegisterNewBall(Ball ball)
    {
        AllBalls.Remove(ball);
    }
}