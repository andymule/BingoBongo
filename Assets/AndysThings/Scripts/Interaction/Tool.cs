using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [SerializeField] protected GameObject highlight;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Tool");
        tag = "ToolOnBelt";
        highlight.SetActive(false);
    }

    public void Select()
    {
        highlight.SetActive(true);
    }

    public void DeSelect()
    {
        highlight.SetActive(false);
    }
}