using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes a master audiosource and manages how it should be played back, including spatial vs stereo
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AndyMusicSystem : MonoBehaviour
{
    private AudioSource _masterSong; // the song that is "broadcast" to all emitters

    private List<AndyMusicSpatialEmitter>
        _allAudioEmitters = new List<AndyMusicSpatialEmitter>(); // emitters register themselves at spawntime into this list

    [SerializeField] private AndyMusicStereoEmitter stereoEmitter; // one of these is manually in scene always
    public bool isPlaying = true;

    private float _spatialBlend = 0; // we can fade between the two modes with this . 1 is spatial, 0 is stereo
    private readonly float _blendTime = .55f; // how long the crossfade takes b/w spatial and stereo

    public enum MusicMode
    {
        Stereo,
        Spatial
    }

    [SerializeField] private MusicMode musicMode = MusicMode.Stereo;

    private void Awake()
    {
        _masterSong = GetComponent<AudioSource>();
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

        // first speaker automatically plays out in the world
        if (_allAudioEmitters.Count == 1)
            SetMode(MusicMode.Spatial);
    }

    public void RemoveEmitterFromList(AndyMusicSpatialEmitter newEmitter)
    {
        _allAudioEmitters.Remove(newEmitter);
    }

    public void Play()
    {
        _masterSong.Play();
    }

    public void Pause()
    {
        _masterSong.Pause();
    }

    // public void Stop()
    // {
    //     _masterSong.Stop();
    // }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPlaying)
        {
            Array.Clear(data, 0, data.Length); // the music system manager doesn't play anything itself
            return;
        }

        GiveDataToChildEmitters(data, _spatialBlend);
        stereoEmitter.InjectAudioData(data, 1 - _spatialBlend);

        Array.Clear(data, 0, data.Length); // the music system manager doesn't play anything itself
    }

    private void GiveDataToChildEmitters(float[] data, float level)
    {
        foreach (AndyMusicSpatialEmitter spatialEmitter in _allAudioEmitters)
        {
            spatialEmitter.InjectAudioData(data, level);
        }
    }
}