using System.Text;
using TMPro;
using UnityEngine;

public class PersonnelFilePanel : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    public static bool IsAnyPersonnelFileOpen { get; private set; }
    private static PersonnelFilePanel openPanel;

    private void Awake()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void Show(PersonnelFileRecord record)
    {
        if (record == null)
        {
            return;
        }

        if (titleText != null)
        {
            titleText.text = record.Title;
        }

        if (contentText != null)
        {
            contentText.text = BuildFileText(record);
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        openPanel = this;
        IsAnyPersonnelFileOpen = true;
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
        AddLine(builder, "Position", record.Position);
        AddLine(builder, "Service", record.ServiceLength);
        AddLine(builder, "Employee ID", record.EmployeeId);
        AddLine(builder, "Clearance", record.ClearanceLevel);
        AddLine(builder, "Status", record.Status);

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

    public bool IsOpen => panelRoot != null && panelRoot.activeSelf;
}
