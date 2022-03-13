using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Root class for audio emitters in the scene
/// Not a true interface, but not meant to be used standalone
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class IAndyMusicEmitter : MonoBehaviour
{
    private int _audioFrameSize;
    private float[] _audioDataToPlay;
    public bool audioMuted; // an emitter might remain in scene but not be audio active (muted). spawns in unmuted 
    private string _debugGameObjectName;
    [HideInInspector] public bool initialialized; // audio thread can try to jump ahead while object is still creating, so we have to check if ready
    
    void Awake()
    {
        AudioSettings.GetDSPBufferSize(out _audioFrameSize, out int _);
        _audioDataToPlay = new float[_audioFrameSize * 2]; // we know our system is stereo, Unity thinks it's 4 channels some times
        _debugGameObjectName = name;
        initialialized = true;
    }

    /// <summary>
    /// Copies data from MusicSystem into specific emitters. Will incur 1 frame of audio latency for buffer transfer.
    /// can control volume from level scalar
    /// </summary>
    /// <param name="data"></param>
    /// <param name="level"></param>
    /// <exception cref="InvalidDataException"></exception>
    public void InjectAudioData(float[] data, float level)
    {
        if (_audioDataToPlay == null)
        {
            Debug.LogWarning("Tried to InjectAudioData before ready. Can happen during init.");
            return;
        }

        if (data.Length != _audioDataToPlay.Length)
            throw new InvalidDataException("InjectAudioData: Audio buffer size mismatch");

        for (int i = 0; i < data.Length; i++)
        {
            _audioDataToPlay[i] = data[i] * level;
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (audioMuted)
        {
            Array.Clear(data, 0, data.Length);
            return;
        }

        if (_audioDataToPlay == null)
        {
            Debug.LogWarning("Tried to play emitter data in OnAudioFilterRead before ready. Can happen during init.");
            return;
        }

        // float runningTotal = 0;
        // foreach (float f in _audioDataToPlay)
        // {
        //     runningTotal += f;
        // }
        // if (runningTotal == 0)
        //     Debug.LogWarning(_debugGameObjectName + ": empty audio");
        

        if (data.Length != _audioDataToPlay.Length)
            throw new InvalidDataException("OnAudioFilterRead: Audio buffer size mismatch");
        Array.Copy(_audioDataToPlay, data, data.Length);
        Array.Clear(_audioDataToPlay, 0, _audioDataToPlay.Length); // eat our own data after we play it
    }
}