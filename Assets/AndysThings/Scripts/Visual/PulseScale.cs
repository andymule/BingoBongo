using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseScale : MonoBehaviour
{
    private Vector3 _startingScale;
    [SerializeField] private float pulseScale = 0.025f;
    [SerializeField] private float pulseSpeed = 2f;

    void Start()
    {
        _startingScale = this.transform.localScale;
    }

    void Update()
    {
        this.transform.localScale = _startingScale + new Vector3(1, 1, 1) * (pulseScale * Mathf.Sin(Time.time * pulseSpeed));
    }
}