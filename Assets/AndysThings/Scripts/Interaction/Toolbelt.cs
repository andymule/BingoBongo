using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toolbelt : MonoBehaviour
{
    [SerializeField] private List<Tool> toolOnBelt;

    void Start()
    {
        DeselectAll();
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