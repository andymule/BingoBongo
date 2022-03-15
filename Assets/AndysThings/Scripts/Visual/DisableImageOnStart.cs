using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableImageOnStart : MonoBehaviour
{
    void Start()
    {
        GetComponent<Image>().enabled = false;
        Destroy(this);
    }
}
