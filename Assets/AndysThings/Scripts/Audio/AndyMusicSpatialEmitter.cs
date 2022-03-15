using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A point source spatial audio emitter that is playing your music in your environment.
/// Because user can manually spawns these in, it self-registers to the music system
/// </summary>
public class AndyMusicSpatialEmitter : IAndyMusicEmitter
{
    private AndyMusicSystem _musicSystem; // find the master system instance at start so we can register/remove

    void Start()
    {
        _musicSystem = FindObjectOfType<AndyMusicSystem>();
        _musicSystem.RegisterNewEmitterToList(this);
    }

    private void OnDestroy()
    {
        _musicSystem.RemoveEmitterFromList(this);
    }
}
