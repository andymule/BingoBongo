using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [SerializeField] protected GameObject highlight;
    private Toolbelt _toolbelt;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Tool");
        tag = "ToolOnBelt";
        highlight.SetActive(false);
        _toolbelt = FindObjectOfType<Toolbelt>();
        _toolbelt.RegisterNewTool(this);
    }

    private void OnDestroy()
    {
        if (_toolbelt != null)
            _toolbelt.RemoveOldTool(this);
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