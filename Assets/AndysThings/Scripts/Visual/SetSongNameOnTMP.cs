using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class SetSongNameOnTMP : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    private AndyMusicSystem _andyMusicSystem;

    void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _andyMusicSystem = FindObjectOfType<AndyMusicSystem>();
    }

    void Update()
    {
        if (!_andyMusicSystem.isPlaying
            || _andyMusicSystem == null
            || _andyMusicSystem._masterSong == null
            || _andyMusicSystem._masterSong.clip == null)
        {
            _textMeshPro.text = "";
        }
        else
        {
            _textMeshPro.text = _andyMusicSystem._masterSong.clip.name;
        }
    }
}