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
        toolToSelect.GetComponent<Tool>().Select();
    }

    private void DeselectAll()
    {
        foreach (var o in toolOnBelt)
        {
            o.DeSelect();
        }
    }
}