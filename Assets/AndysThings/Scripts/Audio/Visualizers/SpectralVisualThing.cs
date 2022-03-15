using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectralVisualThing : MonoBehaviour
{
    private AudioSource _audioSource;
    private float[] _spectrum;
    [SerializeField] private GameObject oneSlice; // the thing we copy to make vizualizer
    private List<Slice> _allSlices = new List<Slice>(); // stores all parts of the visualizer 
    private readonly int bins = 64; // must be power of two for unity's FFT
    private readonly float gapSize = 0.006f;
    private float _startSizeX;
    private float _startSizeHeight;
    public FFTWindow window = FFTWindow.Blackman;
    
    // these ignores declare parts of the spectrum that we don't want to vizualize (very top and bottom, and some gaps for separation)
    private readonly int ignoreBottom = 4;
    private readonly int ignoreTop = 30;
    private readonly int ignoreEvery = 2;
    
    [SerializeField] private float sizeSensitivity = 1.5f;
    [SerializeField] private float colorSensitivity = 100f;

    private readonly Color transYellow = new Color(1, 1, 0, .3f);
    private readonly Color transGrey = new Color(.5f, .5f, .5f, .15f);

    void Start()
    {
        _spectrum = new float[bins];
        _audioSource = GetComponent<AudioSource>();

        // look around everywhere for an AudioSource. Volatile code but we're prototyping
        if (_audioSource == null)
        {
            _audioSource = GetComponentInChildren<AudioSource>();
        }

        if (_audioSource == null)
        {
            _audioSource = transform.parent.GetComponent<AudioSource>();
        }

        // spawns in our vizualizer from the one slice prefab
        for (int i = 0; i < bins - ignoreBottom - ignoreTop; i += ignoreEvery)
        {
            GameObject thisSliceGO = Instantiate(oneSlice, transform);
            thisSliceGO.transform.position = oneSlice.transform.position + new Vector3(0, gapSize, 0) * i;
            // adds new slice to list for quick access of components we care about
            _allSlices.Add(new Slice
            {
                _transform = thisSliceGO.transform,
                _renderer = thisSliceGO.GetComponent<Renderer>()
            });
        }

        _startSizeX = oneSlice.transform.localScale.x; // assumes square things
        _startSizeHeight = oneSlice.transform.localScale.y;
        Destroy(oneSlice); // we don't use the original slice
    }

    void Update()
    {
        _audioSource.GetSpectrumData(_spectrum, 0, window);
        for (int i = 0; i < _allSlices.Count; i++)
        {
            var thisPoint = _spectrum[i * ignoreEvery + ignoreBottom];

            _allSlices[i]._transform.localScale = new Vector3(
                x: _startSizeX + thisPoint * sizeSensitivity,
                y: _startSizeHeight,
                z: _startSizeX + thisPoint * sizeSensitivity);

            _allSlices[i]._renderer.material.color = Color.Lerp(transGrey, transYellow, thisPoint * colorSensitivity);
        }
    }

    internal class Slice
    {
        internal Renderer _renderer;
        internal Transform _transform;
    }
}