using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Piping.Trinary.Components;
using Content.Shared.Database;
using Content.Shared.Examine;

namespace Content.Shared.Atmos.Piping.Trinary.EntitySystems;

public abstract partial class SharedGasFilterSystem : EntitySystem
{
    [Dependency] private ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private SharedAtmosphereSystem _atmosphereSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GasFilterComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<GasFilterComponent> ent, ref ExaminedEvent args)
    {
        if (Loc.TryGetString("gas-volume-pump-system-examined",
                out var transferRateStr,
                ("statusColor", "lightblue"),
                ("rate", ent.Comp.TransferRate.ToString("G"))
            ))
        {
            args.PushMarkup(transferRateStr);
        }

        // funky - start
        var gasName = Loc.GetString("comp-gas-filter-ui-filter-gas-none");
        if (ent.Comp.FilteredGases.Count > 0)
        {
            var gases = ent.Comp.FilteredGases.Select(gasId => _atmosphereSystem.GetGas(gasId).Name);
            gasName = string.Join(", ", gases);
        }
        // funky - end

        if (Loc.TryGetString("comp-gas-filter-filtered-gas-examine",
                out var filteredGasStr,
                ("statusColor", "lightblue"),
                ("filteredGas", gasName)
            ))
        {
            args.PushMarkup(filteredGasStr);
        }
    }

    [SubscribeLocalEvent]
    private void OnToggleStatusMessage(Entity<GasFilterComponent> ent, ref GasFilterToggleStatusMessage args)
    {
        ent.Comp.Enabled = args.Enabled;
        _adminLogger.Add(LogType.AtmosPowerChanged, LogImpact.Medium,
            $"{ToPrettyString(args.Actor):player} set the power on {ToPrettyString(ent.Owner):device} to {args.Enabled}");

        DirtyField(ent.Owner, ent.Comp, nameof(GasFilterComponent.Enabled));
        UpdateUi(ent);
        UpdateAppearance(ent);
    }

    [SubscribeLocalEvent]
    private void OnTransferRateChangeMessage(Entity<GasFilterComponent> ent, ref GasFilterChangeRateMessage args)
    {
        ent.Comp.TransferRate = Math.Clamp(args.Rate, 0f, ent.Comp.MaxTransferRate);
        _adminLogger.Add(LogType.AtmosVolumeChanged, LogImpact.Medium,
            $"{ToPrettyString(args.Actor):player} set the transfer rate on {ToPrettyString(ent.Owner):device} to {args.Rate}");

        DirtyField(ent.Owner, ent.Comp, nameof(GasFilterComponent.TransferRate));
        UpdateUi(ent);
    }

    [SubscribeLocalEvent]
    private void OnSelectGasMessage(Entity<GasFilterComponent> ent, ref GasFilterSelectGasesMessage args)
    {
        // funky - start
        if (args.Gases.Count > 0)
        {
            if (args.Gases.Any(gas => !Enum.IsDefined(typeof(Gas), gas)))
            {
                var invalidIds = string.Join(", ", args.Gases.Where(gas => !Enum.IsDefined(typeof(Gas), gas)));
                Log.Warning($"{ToPrettyString(ent.Owner)} received GasFilterSelectGasMessage with invalid IDs: {invalidIds}");
                return;
            }

            ent.Comp.FilteredGases = args.Gases;
            _adminLogger.Add(LogType.AtmosFilterChanged, LogImpact.Medium,
                $"{ToPrettyString(args.Actor):player} set the filter on {ToPrettyString(ent.Owner):device} to {string.Join(", ", args.Gases)}");
        }
        else
        {
            ent.Comp.FilteredGases = new List<Gas>();
            _adminLogger.Add(LogType.AtmosFilterChanged, LogImpact.Medium,
                $"{ToPrettyString(args.Actor):player} set the filter on {ToPrettyString(ent.Owner):device} to none");
        }

        DirtyField(ent.Owner, ent.Comp, nameof(GasFilterComponent.FilteredGases));
        // funky - end
        UpdateUi(ent);
    }

    protected void UpdateAppearance(Entity<GasFilterComponent> ent)
    {
        _appearance.SetData(ent, FilterVisuals.Enabled, ent.Comp.Enabled);
    }

    protected virtual void UpdateUi(Entity<GasFilterComponent> ent)
    {
    }
}
