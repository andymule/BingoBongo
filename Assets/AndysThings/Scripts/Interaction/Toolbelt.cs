using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        DeselectAll();
        MusicTool thisTool = toolToSelect.GetComponent<MusicTool>();
        if (thisTool != null)
        {
            thisTool.Select();
        }
        else
        {
            toolToSelect.GetComponent<Tool>().Select();
        }
    }

    public void DeselectAll()
    {
        foreach (var o in toolOnBelt)
        {
            o.DeSelect();
        }
    }
}