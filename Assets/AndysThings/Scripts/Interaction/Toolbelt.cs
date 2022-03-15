using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

/// <summary>
/// Tracks all tools in scene. Holds such useful tools.
/// Tools register themselves when spawned in.
/// ForwardRayManager triggers the SelectTool function on valid gaze.
/// SelectTool notifies each Tool of their selection state. 
/// </summary>
public class Toolbelt : MonoBehaviour
{
    [SerializeField] private List<Tool> toolOnBelt;

    public void RegisterNewTool(Tool newTool)
    {
        toolOnBelt.Add(newTool);
    }

    public void RemoveOldTool(Tool oldTool)
    {
        toolOnBelt.Remove(oldTool);
    }

    public void SelectTool(GameObject toolToSelect)
    {
        var thisTool = toolToSelect.GetComponent<Tool>();
        foreach (var tool in toolOnBelt.Where(tool => tool != thisTool))
        {
            tool.DeSelect();
        }
        thisTool.Select();
    }
}