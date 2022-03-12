using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectralVisualThing : MonoBehaviour
{
    private AudioSource _audioSource;
    private float[] _spectrum;
    [SerializeField] private GameObject oneSlice;
    private List<GameObject> _allSlices = new List<GameObject>(); // stores all parts of the visualizer 
    private readonly int slicesToTake = 64; // must be power of two
    private readonly float gapSize = 0.006f;
    private float _startSizeX;
    private float _startSizeHeight;
    public FFTWindow window = FFTWindow.Hamming;
    private readonly int ignoreBottom = 4;
    private readonly int ignoreTop = 30;
    private readonly int ignoreEvery = 2;

    void Start()
    {
        _spectrum = new float[slicesToTake];
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

        for (int i = 0; i < slicesToTake - ignoreBottom - ignoreTop; i+= ignoreEvery)
        {
            GameObject thisSlice = Instantiate(oneSlice, transform);
            thisSlice.transform.position = oneSlice.transform.position + new Vector3(0, gapSize, 0) * i;
            _allSlices.Add(thisSlice);
        }

        _startSizeX = oneSlice.transform.localScale.x;
        _startSizeHeight = oneSlice.transform.localScale.y;
        Destroy(oneSlice);
    }

    void Update()
    {
        _audioSource.GetSpectrumData(_spectrum, 0, window);
        for (int i = 0; i < _allSlices.Count; i++)
        {
            _allSlices[i].transform.localScale = new Vector3(_startSizeX + _spectrum[i*ignoreEvery + ignoreBottom], _startSizeHeight,
                _startSizeX + _spectrum[i*ignoreEvery + ignoreBottom]);
        }
    }
}