using Content.Server.Wires;
using Content.Shared._Funkystation.Documents.Components;
using Content.Shared.VendingMachines;
using Content.Shared.Wires;

namespace Content.Server._Funkystation.Documents.Wires;

public sealed partial class DocumentPrinterManagerWireAction : BaseToggleWireAction
{
    public override string Name { get; set; } = "wire-name-document-printer-manager";
    public override Color Color { get; set; } = Color.Purple;
    public override object? StatusKey { get; } = ContrabandWireKey.StatusKey;

    public override void ToggleValue(EntityUid owner, bool setting)
    {
        if (EntityManager.TryGetComponent<DocumentPrinterComponent>(owner, out var comp))
            comp.ManagerWireCut = !setting;

        EntityManager.System<DocumentPrinterSystem>().RefreshUi(owner);
    }

    public override bool GetValue(EntityUid owner)
    {
        return EntityManager.TryGetComponent<DocumentPrinterComponent>(owner, out var comp)
            && !comp.ManagerWireCut;
    }

    public override StatusLightState? GetLightState(Wire wire)
    {
        return EntityManager.TryGetComponent<DocumentPrinterComponent>(wire.Owner, out var comp) && comp.ManagerWireCut
            ? StatusLightState.BlinkingSlow
            : StatusLightState.On;
    }
}
