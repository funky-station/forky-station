using Content.Server._Funkystation.TraderCargo.Components;
using Content.Server._Funkystation.TraderCargo.Events;
using Content.Shared._Funkystation.TraderCargo.Components;
using Content.Shared._Funkystation.TraderCargo.Systems;
using Content.Shared.UserInterface;

using Content.Server.Station.Systems;

namespace Content.Server._Funkystation.TraderCargo.Systems;

public sealed partial class FTLCommsComputerSystem : SharedFTLCommsComputerSystem
{
    [Dependency] private StationSystem _station = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FTLCommsComputerComponent, AfterActivatableUIOpenEvent>(OnActivate);
    }

    private void OnActivate(EntityUid ent, FTLCommsComputerComponent comp, AfterActivatableUIOpenEvent args)
    {
        var station = _station.GetOwningStation(ent);

        if (station == null || !TryComp<StationTraderComponent>(station, out var traderComp))
            return;


    }
}
