using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTool : Tool
{
    [SerializeField] private float timeToStayActiveAfterGaze = 5f;
    private float timeActiveRemaining = 0f;
    private MicrophoneHandling _voiceInput;
    private bool isPlacingSpeaker = false;
    private AndyMusicSystem _andyMusicSystem;

    private void Start()
    {
        _voiceInput = FindObjectOfType<MicrophoneHandling>();
        _andyMusicSystem = FindObjectOfType<AndyMusicSystem>();
    }

    public new void Select()
    {
        highlight.SetActive(true);
        timeActiveRemaining = timeToStayActiveAfterGaze;
        _voiceInput.OnStart();
    }

    private void Update()
    {
        if (!isPlacingSpeaker) // pause timer while placing speaker
            timeActiveRemaining -= Time.deltaTime;

        timeActiveRemaining = Mathf.Max(timeActiveRemaining, 0);
        if (timeActiveRemaining <= 0)
        {
            _voiceInput.OnStop(); // TODO hide and show visual state too
        }
        else // voice is active
        {
            if (_voiceInput.ContainsPhrase("Play Music") || _voiceInput.ContainsPhrase("Start Music"))
            {
                _andyMusicSystem.Play();
                print("PLAY MUSIC");
                ResetVoice();
            }

            if (_voiceInput.ContainsPhrase("Stop Music") || _voiceInput.ContainsPhrase("Pause Music"))
            {
                _andyMusicSystem.Pause();
                print("STOP MUSIC");
                ResetVoice();
            }

            if (_voiceInput.ContainsPhrase("Make Emitter") || _voiceInput.ContainsPhrase("Make Speaker"))
            {
                isPlacingSpeaker = true;
                print("MAKING SPEAKER");
                ResetVoice();
            }

            if (_voiceInput.ContainsPhrase("Place Speaker") || _voiceInput.ContainsPhrase("Set There"))
            {
                isPlacingSpeaker = false;
                print("PLACE SPEAKER");
                ResetVoice();
            }

            if (_voiceInput.ContainsPhrase("Play Stereo"))
            {
                _andyMusicSystem.SetMode(AndyMusicSystem.MusicMode.Stereo);
                print("PLAY STEREO");
                ResetVoice();
            }

            if (_voiceInput.ContainsPhrase("Play Spatial"))
            {
                _andyMusicSystem.SetMode(AndyMusicSystem.MusicMode.Spatial);
                print("PLAY SPATIAL");
                ResetVoice();
            }
        }
    }

    private void ResetVoice()
    {
        _voiceInput.OnStop();
        _voiceInput.ClearText();
        _voiceInput.OnStart();
    }
}