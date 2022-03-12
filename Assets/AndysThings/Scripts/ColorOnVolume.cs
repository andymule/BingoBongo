using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies a basic preset color to mesh based on output level
/// </summary>
public class ColorOnVolume : MonoBehaviour
{
    readonly float AnimationTime = 0.045f;
    [SerializeField] private float sensitivityMultiplier = 14f;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color startingColor = Color.white;
    private Renderer _renderer;
    private int _frameSize = 1024; // overridden in start from audio settings
    private AudioSource _audioSource;
    private float[] _samples;

    void Start()
    {
        _renderer = GetComponent<Renderer>();

        AudioSettings.GetDSPBufferSize(out _frameSize, out int _);
        _samples = new float[_frameSize];

        // look around everywhere for an AudioSource. Volatile code but we're prototyping
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = GetComponentInChildren<AudioSource>();
        }

        if (_audioSource == null)
        {
            _audioSource = transform.parent.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        _audioSource.GetOutputData(_samples, 0); // lets just sample the first channel first
        float runningAvg = 0;
        for (int i = 0; i < _frameSize; i++)
        {
            runningAvg += _samples[i] * _samples[i];
        }

        // _audioSource.GetOutputData(_samples, 1); // lets do second channel now cool
        // for (int i = 0; i < _frameSize; i++)
        // {
        //     runningAvg += _samples[i] * _samples[i];
        // }

        // start w Root Mean Square but we are scaling it here to make the colors work well for us
        float colorAmount = Mathf.Sqrt(runningAvg / _frameSize) * sensitivityMultiplier;
        colorAmount = Mathf.Clamp01(colorAmount);

        // var _targetColor = new Color(colorAmount * highlightColor.r, colorAmount * highlightColor.g, colorAmount * highlightColor.b);
        var thisFrameTarget = Color.Lerp(startingColor, highlightColor, colorAmount);
        _renderer.material.color = thisFrameTarget; //Color.Lerp(_renderer.material.color, _targetColor, Time.deltaTime * (1 / AnimationTime));
    }
}