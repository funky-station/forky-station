using System.Linq;
using Content.Shared.Medical.SuitSensor;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Medical.CrewMonitoring;

public sealed partial class CrewMonitoringWindow
{
    private List<SuitSensorStatus> _uniqueSensors = [];

    // Gets called each time the search bar changes
    private void OnSearchLineTextChanged(LineEdit.LineEditEventArgs args)
    {
        RebuildSensorsTable();
    }

    private void RebuildSensorsTable()
    {
        ClearOutDatedData();

        var orderedSensors = _uniqueSensors
            .OrderBy(n => n.Name)
            .ThenBy(j => j.Job)
            .ToList();

        var departments = _uniqueSensors
            .SelectMany(sensor => sensor.JobDepartments)
            .Distinct()
            .OrderBy(department => department);

        foreach (var department in departments)
        {
            var departmentSensors = orderedSensors
                .Where(sensor => sensor.JobDepartments.Contains(department))
                .ToList();

            AddDepartmentToSensorsTable(department, departmentSensors);
        }

        var remainingSensors = orderedSensors
            .Where(sensor => sensor.JobDepartments.Count == 0)
            .ToList();

        AddDepartmentToSensorsTable(Loc.GetString("crew-monitoring-ui-no-department-label"), remainingSensors);
    }

    // Appends the department and its sensors to the sensors table
    private void AddDepartmentToSensorsTable(string departmentName, List<SuitSensorStatus> departmentSensors)
    {

        // Checks if at least one sensor inside the department matches the filter on the search bar
        if (departmentSensors.Count == 0 || !departmentSensors.Any(MatchesSearchFilter))
            return;

        if (SensorsTable.ChildCount > 0)
        {
            SensorsTable.AddChild(new Control
            {
                SetHeight = 20,
            });
        }

        var departmentLabel = new RichTextLabel
        {
            Margin = new Thickness(10, 0),
            HorizontalExpand = true,
        };

        departmentLabel.SetMessage(departmentName);
        departmentLabel.StyleClasses.Add("font-large");

        SensorsTable.AddChild(departmentLabel);
        PopulateDepartmentList(departmentSensors);
    }

    private static List<SuitSensorStatus> GetUniqueSensorsList(List<SuitSensorStatus> allSensors)
    {
        Dictionary<NetEntity, SuitSensorStatus> uniqueSensorsMap = new();
        foreach (var sensor in allSensors)
        {
            if (uniqueSensorsMap.TryGetValue(sensor.OwnerUid, out var existingSensor))
            {
                // Skip if we already have a sensor with more data for this entity
                if (existingSensor.Coordinates != null && sensor.Coordinates == null)
                    continue;

                if (existingSensor.DamagePercentage != null && sensor.DamagePercentage == null)
                    continue;
            }

            uniqueSensorsMap[sensor.OwnerUid] = sensor;
        }

        return uniqueSensorsMap.Values.ToList();
    }

    // Search filter for the search bar
    // Matches the filtering logic used upstream, extracted in its own method
    private bool MatchesSearchFilter(SuitSensorStatus sensor)
    {
        return string.IsNullOrEmpty(SearchLineEdit.Text)
               || sensor.Name.Contains(
                   SearchLineEdit.Text,
                   StringComparison.CurrentCultureIgnoreCase)
               || sensor.Job.Contains(
                   SearchLineEdit.Text,
                   StringComparison.CurrentCultureIgnoreCase);
    }
}
