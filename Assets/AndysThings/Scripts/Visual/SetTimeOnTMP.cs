using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTimeOnTMP : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _textMeshPro.text = $"{DateTime.Now:HH:mm}";
    }
}
