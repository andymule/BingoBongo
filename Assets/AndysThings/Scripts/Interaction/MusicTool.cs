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
    private AnchorCreator2 _anchorCreator2;
    [SerializeField] private GameObject confirmationFlashOverlay;
    private GameObject currentSelectedObject;
    private bool isDeletable; // dont delete main music tool, but others can go as they please

    
    // we just got a command and need to reset the voice text buffer but it's still kind of trying to figure out whats up
    // so it keeps returning things and can cause re-triggers
    // this float stops the retriggering by not allowing ANYTHING for a bit while clearing
    [SerializeField] private float minTimeBetweenCommands = 1.35f;
    private float timeBetweenCommandsTimer; // actually does the countdown for above

    private void Start()
    {
        _voiceInput = FindObjectOfType<MicrophoneHandling>();
        _andyMusicSystem = FindObjectOfType<AndyMusicSystem>();
        _anchorCreator2 = FindObjectOfType<AnchorCreator2>();
        if (confirmationFlashOverlay == null)
            confirmationFlashOverlay = GameObject.FindWithTag("ConfirmationOverlay");
    }

    public new void Select()
    {
        highlight.SetActive(true);
        timeActiveRemaining = timeToStayActiveAfterGaze;
    }

    private void Update()
    {
        if (!isPlacingSpeaker) // timer pauses while placing speaker
            timeActiveRemaining -= Time.deltaTime;
        timeActiveRemaining = Mathf.Max(timeActiveRemaining, 0);

        timeBetweenCommandsTimer -= Time.deltaTime;
        timeBetweenCommandsTimer = Mathf.Max(timeBetweenCommandsTimer, 0);
        if (timeBetweenCommandsTimer > 0)
        {
            _voiceInput.ClearText(); // stupid, but we keep spamming calls clear this whole time  
        }
        else if (timeActiveRemaining > 0)
        {
            CheckVoiceCommands();
        }
    }

    private void CheckVoiceCommands()
    {
        if (_voiceInput.ContainsOneOfThesePhrases("Play Music", "Start", "Latin"))
        {
            _andyMusicSystem.PlayLatin();
            ResetVoiceTextBuffer();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Jazz"))
        {
            _andyMusicSystem.PlayJazz();
            ResetVoiceTextBuffer();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Play Love", "Do Love", "Play Lovesong", "Play Love Song"))
        {
            _andyMusicSystem.PlayLove();
            ResetVoiceTextBuffer();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Play") && !_andyMusicSystem.isPlaying) //default music case, or pause resume
        {
            if (_andyMusicSystem._masterSong.clip != null)
                _andyMusicSystem.Play(); // a song is already playing, so resume
            else
                _andyMusicSystem.PlayLatin(); // nothing playing yet, so play default song
            ResetVoiceTextBuffer();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Pause", "Stop"))
        {
            _andyMusicSystem.Pause();
            ResetVoiceTextBuffer();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Create", "Make", "Spawn"))
        {
            isPlacingSpeaker = true;
            _anchorCreator2.EnterPlacementMode();
            ResetVoiceTextBuffer();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Remove", "Delete"))
        {
            ResetVoiceTextBuffer();
            if (isDeletable)
                Destroy(this.gameObject);
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Set", "Place"))
        {
            isPlacingSpeaker = false;
            _anchorCreator2.PlaceThereNOW();
            ResetVoiceTextBuffer();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Stereo", "Headset", "Headphones", "Airpods", "Earbuds", "Airpod", "Earbud", "Headphone"))
        {
            _andyMusicSystem.SetMode(AndyMusicSystem.MusicMode.Stereo);
            ResetVoiceTextBuffer();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Spatial", "World", "Outside"))
        {
            _andyMusicSystem.SetMode(AndyMusicSystem.MusicMode.Spatial);
            ResetVoiceTextBuffer();
        }
    }

    private void ResetVoiceTextBuffer()
    {
        StopAllCoroutines();
        StartCoroutine(FlashNFade());
        _voiceInput.OnStop();
        _voiceInput.ClearText();
        _voiceInput.OnStart();
        timeBetweenCommandsTimer = minTimeBetweenCommands;
    }

    private IEnumerator FlashNFade()
    {
        confirmationFlashOverlay.SetActive(true);
        yield return new WaitForSeconds(.3f);
        confirmationFlashOverlay.SetActive(false);
    }
}