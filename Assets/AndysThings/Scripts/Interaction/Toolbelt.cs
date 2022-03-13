using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toolbelt : MonoBehaviour
{
    [SerializeField] private List<Tool> toolOnBelt;
    private Tool currentSelectedTool;

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
        if (thisTool == currentSelectedTool)
            return;
        DeselectAll();
        MusicTool thisMusicTool = toolToSelect.GetComponent<MusicTool>();
        if (thisMusicTool != null)
        {
            thisMusicTool.Select();
        }
        else
        {
            thisTool.Select();
        }

        currentSelectedTool = thisTool;
    }

    public void DeselectAll()
    {
        foreach (var o in toolOnBelt)
        {
            o.DeSelect();
        }

        currentSelectedTool = null;
    }
}