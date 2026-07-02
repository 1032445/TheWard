using System;
using UnityEngine;

[Serializable]
public class LogbookPage
{
    [SerializeField] private string title = "HX-7B DISTRIBUTION LOG";
    [SerializeField] private string entry;
    [SerializeField] private string date;
    [SerializeField] private string system;
    [SerializeField] private string area;
    [SerializeField] private string doseCycle;
    [SerializeField] private string batch;
    [SerializeField] private string machineStatus;
    [SerializeField] private string facilityResponse;
    [TextArea(4, 10)]
    [SerializeField] private string notes;

    public string Title => title;
    public string Entry => entry;
    public string Date => date;
    public string System => system;
    public string Area => area;
    public string DoseCycle => doseCycle;
    public string Batch => batch;
    public string MachineStatus => machineStatus;
    public string FacilityResponse => facilityResponse;
    public string Notes => notes;
}
