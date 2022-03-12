using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetDateOnTMP : MonoBehaviour
{
    void Start()
    {
        var today = DateTime.Today;
        GetComponent<TextMeshProUGUI>().text = $"{today:MMMM} {today.Day}\n{today.Year}";
    }
}
