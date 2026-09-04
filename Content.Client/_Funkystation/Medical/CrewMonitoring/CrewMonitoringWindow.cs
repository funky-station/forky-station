using System.Linq;
using Content.Shared.Medical.SuitSensor;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Medical.CrewMonitoring;

public sealed partial class CrewMonitoringWindow
{
    private List<SuitSensorStatus> _uniqueSensors = [];

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

            // Checks if the department has at least one sensor matching the search filter
            // if not, don't add the department label
            if (departmentSensors.Count == 0 || !departmentSensors.Any(MatchesSearchFilter))
                continue;

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

            departmentLabel.SetMessage(department);
            departmentLabel.StyleClasses.Add("font-large");

            SensorsTable.AddChild(departmentLabel);
            PopulateDepartmentList(departmentSensors);
        }

        var remainingSensors = orderedSensors
            .Where(sensor => sensor.JobDepartments.Count == 0)
            .ToList();

        if (remainingSensors.Count == 0 || !remainingSensors.Any(MatchesSearchFilter))
            return;

        if (SensorsTable.ChildCount > 0)
        {
            SensorsTable.AddChild(new Control
            {
                SetHeight = 20,
            });
        }

        var noDepartmentLabel = new RichTextLabel
        {
            Margin = new Thickness(10, 0),
            HorizontalExpand = true,
        };

        noDepartmentLabel.SetMessage(Loc.GetString("crew-monitoring-ui-no-department-label"));
        noDepartmentLabel.StyleClasses.Add("font-large");

        SensorsTable.AddChild(noDepartmentLabel);
        PopulateDepartmentList(remainingSensors);
    }

    private static List<SuitSensorStatus> GetUniqueSensorsList(List<SuitSensorStatus> allSensors)
    {
        Dictionary<NetEntity, SuitSensorStatus> uniqueSensorsMap = new();
        foreach (var sensor in allSensors)
        {
            if (uniqueSensorsMap.TryGetValue(sensor.OwnerUid, out var existingSensor))
            {
                // Skip if we already have a sensor with more data for this mob.
                if (existingSensor.Coordinates != null && sensor.Coordinates == null)
                    continue;

                if (existingSensor.DamagePercentage != null && sensor.DamagePercentage == null)
                    continue;
            }

            uniqueSensorsMap[sensor.OwnerUid] = sensor;
        }

        return uniqueSensorsMap.Values.ToList();
    }

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
