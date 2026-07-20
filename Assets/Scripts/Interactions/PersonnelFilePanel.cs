using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersonnelFilePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private TextMeshProUGUI pageText;

    public static bool IsAnyPersonnelFileOpen { get; private set; }
    private static PersonnelFilePanel openPanel;
    private PersonnelFileRecord[] records;
    private int currentRecordIndex;
    private bool isBeingShown;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        openPanel = null;
        IsAnyPersonnelFileOpen = false;
    }

    private void Awake()
    {
        if (panelRoot != null && !isBeingShown)
        {
            panelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOpen || records == null || records.Length <= 1)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            ShowRecord(currentRecordIndex + 1);
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            ShowRecord(currentRecordIndex - 1);
        }
    }

    public void Show(PersonnelFileRecord record)
    {
        if (record == null)
        {
            return;
        }

        Show(new[] { record });
    }

    public void Show(PersonnelFileRecord[] fileRecords)
    {
        if (fileRecords == null || fileRecords.Length == 0)
        {
            return;
        }

        records = fileRecords;
        currentRecordIndex = 0;
        ShowRecord(currentRecordIndex);

        if (panelRoot != null)
        {
            isBeingShown = true;
            panelRoot.SetActive(true);
            isBeingShown = false;
        }

        openPanel = this;
        IsAnyPersonnelFileOpen = true;
    }

    private void ShowRecord(int recordIndex)
    {
        if (records == null || records.Length == 0)
        {
            return;
        }

        currentRecordIndex = Mathf.Clamp(recordIndex, 0, records.Length - 1);
        PersonnelFileRecord record = records[currentRecordIndex];

        if (titleText != null)
        {
            titleText.text = record.Title;
        }

        if (contentText != null)
        {
            contentText.text = BuildFileText(record);
        }

        if (pageText != null)
        {
            pageText.text = $"{currentRecordIndex + 1} / {records.Length}";
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        if (openPanel == this)
        {
            openPanel = null;
        }

        records = null;
        currentRecordIndex = 0;
        IsAnyPersonnelFileOpen = false;
    }

    public static void HideOpenPanel()
    {
        if (openPanel != null)
        {
            openPanel.Hide();
            return;
        }

        IsAnyPersonnelFileOpen = false;
    }

    private string BuildFileText(PersonnelFileRecord record)
    {
        StringBuilder builder = new StringBuilder();

        AddLine(builder, "Name", record.EmployeeName);

        AddSpacing(builder);

        AddLine(builder, "Position", record.Position);
        AddLine(builder, "Service", record.ServiceLength);
        AddLine(builder, "Employee ID", record.EmployeeId);
        AddLine(builder, "Clearance", record.ClearanceLevel);

        AddSpacing(builder);

        AddLine(builder, "Status", record.Status);
        AddLine(builder, "Cycle", record.Cycle);
        AddLine(builder, "Intake Window", record.IntakeWindow);

        AddSpacing(builder);

        AddLine(builder, "Stability", record.Stability);
        AddLine(builder, "External Contact", record.ExternalContact);
        AddLine(builder, "Handler", record.Handler);

        if (!string.IsNullOrWhiteSpace(record.Notes))
        {
            builder.AppendLine();
            builder.AppendLine(record.Notes);
        }

        return builder.ToString();
    }

    private void AddLine(StringBuilder builder, string label, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        builder.Append(label);
        builder.Append(": ");
        builder.AppendLine(value);
    }

    private void AddSpacing(StringBuilder builder)
    {
        if (builder.Length == 0)
        {
            return;
        }

        builder.AppendLine();
    }

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
