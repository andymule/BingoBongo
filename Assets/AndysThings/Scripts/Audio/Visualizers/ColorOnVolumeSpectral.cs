using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOnVolumeSpectral : MonoBehaviour
{
    readonly float AnimationTime = 0.045f;
    [SerializeField] private float lowSens = 1f;
    [SerializeField] private float midSens = 5f;
    [SerializeField] private float hiSens = 10f;
    [SerializeField] private int lowColorIndex = 2;
    [SerializeField] private int midColorIndex = 70;
    [SerializeField] private int highColorIndex = 200;
    private Renderer _renderer;
    private AudioSource _audioSource;
    float[] spectrum = new float[256];
    
    void Start()
    {
        _renderer = GetComponent<Renderer>();

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
        _audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Hanning);

        Color newColor = new Color(spectrum[lowColorIndex]*lowSens, spectrum[midColorIndex]*midSens, spectrum[highColorIndex]*hiSens);
        _renderer.material.color = Color.Lerp(_renderer.material.color, newColor, Time.deltaTime * (1 / AnimationTime));
    }
}