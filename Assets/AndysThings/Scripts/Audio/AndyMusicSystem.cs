using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes a master song and manages how it should be played back on speakers, including spatial vs stereo control
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AndyMusicSystem : MonoBehaviour
{
    public AudioSource _masterSong; // the song that is currently being "broadcast" to all emitters

    [SerializeField] private AudioClip JazzSong;
    [SerializeField] private AudioClip LatinSong;
    [SerializeField] private AudioClip LoveSong;

    // emitters register themselves at spawn-time into this list
    private List<AndyMusicSpatialEmitter> _allAudioEmitters = new List<AndyMusicSpatialEmitter>();

    [SerializeField] private AndyMusicStereoEmitter stereoEmitter; // one of these is manually in scene always
    [HideInInspector] public bool isPlaying = false;
    private float volume = 1.0f; // goes from 0.0 - 1.0, scaled to all recievers

    private float _spatialBlend = 0; // we can fade between the two modes with this . 1 is spatial, 0 is stereo
    private readonly float _blendTime = .55f; // how long the crossfade takes b/w spatial and stereo

    public bool HasSpatialEmitters
    {
        get { return _allAudioEmitters.Count > 0; }
    }

    public enum MusicMode
    {
        Stereo,
        Spatial
    }

    [SerializeField] private MusicMode musicMode = MusicMode.Stereo;

    private void Awake()
    {
        _masterSong = GetComponent<AudioSource>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep; // keep screen on for good music times!
    }

    public void ToggleMode()
    {
        if (musicMode == MusicMode.Spatial)
        {
            musicMode = MusicMode.Stereo;
            StopAllCoroutines();
            StartCoroutine(FadeSpatialBlendTo(0f, _blendTime));
        }
        else
        {
            musicMode = MusicMode.Spatial;
            StopAllCoroutines();
            StartCoroutine(FadeSpatialBlendTo(1f, _blendTime));
        }
    }

    public void SetMode(MusicMode mode)
    {
        if (mode == MusicMode.Spatial)
        {
            musicMode = MusicMode.Spatial;
            StopAllCoroutines();
            StartCoroutine(FadeSpatialBlendTo(1f, _blendTime));
        }
        else
        {
            musicMode = MusicMode.Stereo;
            StopAllCoroutines();
            StartCoroutine(FadeSpatialBlendTo(0f, _blendTime));
        }
    }

    private IEnumerator FadeSpatialBlendTo(float targetBlend, float blendTime)
    {
        float progress = 0;
        float startingBlend = _spatialBlend;
        while (progress < 1)
        {
            progress += Time.deltaTime / blendTime;
            _spatialBlend = Mathf.Lerp(startingBlend, targetBlend, progress);
            yield return null;
        }
    }

    public void RegisterNewEmitterToList(AndyMusicSpatialEmitter newEmitter)
    {
        _allAudioEmitters.Add(newEmitter);

        // first speaker automatically plays out in the world with crossfade to spatial
        if (_allAudioEmitters.Count == 1)
            SetMode(MusicMode.Spatial);
    }

    public void RemoveEmitterFromList(AndyMusicSpatialEmitter newEmitter)
    {
        _allAudioEmitters.Remove(newEmitter);

        // if removed last spatial emitters, fade back to stereo
        if (_allAudioEmitters.Count == 0)
            SetMode(MusicMode.Stereo);
    }

    public void Play()
    {
        _masterSong.Play();
        isPlaying = true;
    }

    public void Pause()
    {
        _masterSong.Pause();
        isPlaying = false;
    }


    public void PlayJazz()
    {
        _masterSong.clip = JazzSong;
        Play();
    }

    public void PlayLatin()
    {
        _masterSong.clip = LatinSong;
        Play();
    }

    public void PlayLove()
    {
        _masterSong.clip = LoveSong;
        Play();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPlaying)
        {
            Array.Clear(data, 0, data.Length);
            return;
        }

        for (int i = 0; i < data.Length; i++)
        {
            data[i] *= volume;
        }

        GiveDataToChildEmitters(data, _spatialBlend);
        stereoEmitter.InjectAudioData(data, 1 - _spatialBlend);

        Array.Clear(data, 0, data.Length); // the music system manager doesn't play anything itself
    }

    private void GiveDataToChildEmitters(float[] data, float level)
    {
        foreach (AndyMusicSpatialEmitter spatialEmitter in _allAudioEmitters)
        {
            if (spatialEmitter.initialialized)
                spatialEmitter.InjectAudioData(data, level);
        }
    }

    public void VolumeDown()
    {
        volume -= .25f;
        volume = Mathf.Clamp01(volume);
    }

    public void VolumeUp()
    {
        volume += .25f;
        volume = Mathf.Clamp01(volume);
    }
}