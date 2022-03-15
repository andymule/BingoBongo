using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// The main interactor in the scene. Tools are always on your toolbelt, but can also exist in the world-locked scene
/// </summary>
public class Tool : MonoBehaviour
{
    [SerializeField] protected GameObject highlight; // highlights while active
    [SerializeField] protected float timeToStayActiveAfterGaze = 5f; // you can look away and still interact for this amount of time after
    protected float _timeActiveRemaining = 0f; // counts down active time remaining on this tool if not actively gazing
    protected Toolbelt _toolbelt; // main toolbelt in scene
    protected bool _isSelected;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Tool");
        tag = "ToolOnBelt";
        highlight.SetActive(false);
        _toolbelt = FindObjectOfType<Toolbelt>();
        _toolbelt.RegisterNewTool(this);
    }

    protected virtual void Update()
    {
        _timeActiveRemaining -= Time.deltaTime;
        _timeActiveRemaining = Mathf.Max(_timeActiveRemaining, 0);
        if (_timeActiveRemaining <= 0)
        {
            // if we haven't gazed at this tool for 5 seconds, it's no longer active
            DeSelect();
        }
    }

    private void OnDestroy()
    {
        if (_toolbelt != null)
            _toolbelt.RemoveOldTool(this);
    }

    public virtual void Select()
    {
        _timeActiveRemaining = timeToStayActiveAfterGaze;
        highlight.SetActive(true);
        _isSelected = true;
    }

    public virtual void DeSelect()
    {
        _timeActiveRemaining = 0; // something else is gazed at -- force interaction timeout for this tool 
        highlight.SetActive(false);
        _isSelected = false;
    }
}