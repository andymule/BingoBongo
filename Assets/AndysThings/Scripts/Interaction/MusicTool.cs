using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Inherits from Tool -- this tool manages music playback control from voice+gaze
/// </summary>
public class MusicTool : Tool
{
    private MicrophoneHandling _voiceInput;
    private AndyMusicSystem _andyMusicSystem;
    private AnchorCreator2 _anchorCreator2; // spawns speakers into the world
    [SerializeField] private GameObject confirmationFlashOverlay; // flashes on valid command
    [SerializeField] private bool isDeletable; // dont delete main tool instances, but smaller clones can die as they please
    [SerializeField] private GameObject rootToDeleteIfDeletable; // in case we want to delete a parent when deleting self
    private bool _isPlacingSpeaker; // flags the system to not timeout voice awareness while placing
    private Image _musicToolActiveAndListening; // icon is active while music tool is listening for commands

    // if we just got a command and need to reset the voice text buffer, it can take a sec to settle on the words
    // so it might keep returning valid word combos and can cause re-triggers
    // this float stops the retriggering by not allowing any voice commands for a bit while the voice buffer
    [SerializeField] private float minTimeBetweenCommands = 1.35f;
    private float _timeBetweenCommandsTimer; // actually does the countdown for above

    private void Start()
    {
        _voiceInput = FindObjectOfType<MicrophoneHandling>();
        _andyMusicSystem = FindObjectOfType<AndyMusicSystem>();
        _anchorCreator2 = FindObjectOfType<AnchorCreator2>();
        _musicToolActiveAndListening = GameObject.FindGameObjectWithTag("MusicIcon").GetComponent<Image>();
        if (confirmationFlashOverlay == null)
            confirmationFlashOverlay = GameObject.FindWithTag("ConfirmationOverlay");
    }

    public override void Select()
    {
        _timeActiveRemaining = TimeToStayActiveAfterGaze;
        if (_isSelected)
        {
            return;
        }

        _isSelected = true;
        ResetVoiceBuffer(); // tries to avoid old words triggering on this new tool gaze interaction
        highlight.SetActive(true);
        _musicToolActiveAndListening.enabled = true;
    }

    protected override void Update()
    {
        if (!_isSelected)
            return;

        if (!_isPlacingSpeaker) // timer pauses while placing speaker
            _timeActiveRemaining -= Time.deltaTime;
        _timeActiveRemaining = Mathf.Max(_timeActiveRemaining, 0);

        _timeBetweenCommandsTimer -= Time.deltaTime;
        _timeBetweenCommandsTimer = Mathf.Max(_timeBetweenCommandsTimer, 0);
        if (_timeBetweenCommandsTimer > 0)
        {
            // don't let us take voice commands during this cooldown phase to prevent double triggers on same keyword in STT buffer
            _voiceInput.ClearText(); // stupid, but we keep spamming calls clear this whole time  
        }
        else if (_timeActiveRemaining > 0)
        {
            // if this tool is most recent tool gazed at and it was within active time window, we're listening for commands
            CheckVoiceCommands();
        }
        else
        {
            // if this tool isn't the most recent one we've gazed at or it's just timed out from not being gazed at for a bit
            DeSelect();
        }
    }

    private void CheckVoiceCommands()
    {
        if (_voiceInput.ContainsOneOfThesePhrases("Latin"))
        {
            _andyMusicSystem.PlayLatin();
            GotValidCommand();
        }
        // else if (_voiceInput.ContainsOneOfThesePhrases("Play A Jazz Song"))
        // {
        //     _andyMusicSystem.PlayJazz();
        //     GotValidCommand();
        // }
        else if (_voiceInput.ContainsOneOfThesePhrases("Lovesong", "Love Song"))
        {
            _andyMusicSystem.PlayLove();
            GotValidCommand();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Play", "Resume") &&
                 !_andyMusicSystem.isPlaying) //default music case, or pause resume
        {
            if (_andyMusicSystem._masterSong.clip != null)
                _andyMusicSystem.Play(); // a song is already playing, so resume
            else
                _andyMusicSystem.PlayJazz(); // nothing playing yet, so play default song
            GotValidCommand();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Pause", "Stop") &&
                 _andyMusicSystem.isPlaying)
        {
            _andyMusicSystem.Pause();
            GotValidCommand();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Remove", "Delete"))
        {
            if (isDeletable)
            {
                Destroy(rootToDeleteIfDeletable);
                GotValidCommand();
            }
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Down", "Lower", "Quieter"))
        {
            _andyMusicSystem.VolumeDown();
            GotValidCommand();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Up", "Raise", "Louder"))
        {
            _andyMusicSystem.VolumeUp();
            GotValidCommand();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Set", "Place"))
        {
            if (!_isPlacingSpeaker)
                return;

            _anchorCreator2.PlaceThereWhenNextHit();
            _isPlacingSpeaker = false;
            GotValidCommand();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Create", "Make", "Spawn"))
        {
            if (_isPlacingSpeaker)
                return;

            _anchorCreator2.EnterPlacementMode();
            _isPlacingSpeaker = true;
            GotValidCommand();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Stereo", "Headset", "Headphones", "Airpods", "Earbuds", "Airpod", "Earbud", "Headphone"))
        {
            _andyMusicSystem.SetMode(AndyMusicSystem.MusicMode.Stereo);
            GotValidCommand();
        }
        else if (_voiceInput.ContainsOneOfThesePhrases("Spatial", "World", "Outside") && _andyMusicSystem.HasSpatialEmitters)
        {
            _andyMusicSystem.SetMode(AndyMusicSystem.MusicMode.Spatial);
            GotValidCommand();
        }
    }

    /// <summary>
    /// Flashes a little overlay on screen to confirm valid word, and clears voice buffer
    /// </summary>
    private void GotValidCommand()
    {
        StopAllCoroutines();
        StartCoroutine(FlashNFade());
        _timeBetweenCommandsTimer = minTimeBetweenCommands;
        ResetVoiceBuffer();
    }

    private void ResetVoiceBuffer()
    {
        // _voiceInput.OnStop();
        _voiceInput.ClearText();
        // _voiceInput.OnStart();
    }

    public override void DeSelect()
    {
        if (!_isSelected)
            return;
        _timeActiveRemaining = 0; // something else is gazed at -- force interaction timeout for this tool 
        highlight.SetActive(false);
        _isSelected = false;
        _musicToolActiveAndListening.enabled = false;
    }

    private IEnumerator FlashNFade()
    {
        try
        {
            confirmationFlashOverlay.SetActive(true);
        }
        catch
        {
        }

        yield return new WaitForSeconds(.3f);
        try
        {
            confirmationFlashOverlay.SetActive(false);
        }
        catch
        {
        }
    }
}