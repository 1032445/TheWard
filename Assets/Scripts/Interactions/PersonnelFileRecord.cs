using System;
using UnityEngine;

[Serializable]
public class PersonnelFileRecord
{
    [SerializeField] private string title = "PERSONNEL FILE";
    [SerializeField] private string employeeName;
    [SerializeField] private string position;
    [SerializeField] private string serviceLength;
    [SerializeField] private string employeeId;
    [SerializeField] private string clearanceLevel;
    [SerializeField] private string status;
    [TextArea(3, 8)]
    [SerializeField] private string notes;

    public string Title => title;
    public string EmployeeName => employeeName;
    public string Position => position;
    public string ServiceLength => serviceLength;
    public string EmployeeId => employeeId;
    public string ClearanceLevel => clearanceLevel;
    public string Status => status;
    public string Notes => notes;
}
