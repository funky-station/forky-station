using Content.Server.Wires;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared._Funkystation.Documents.Components;
using Content.Shared.Access;
using Content.Shared.Wires;

namespace Content.Server._Funkystation.Documents.Wires;

public sealed partial class DocumentPrinterAccessWireAction : BaseToggleWireAction
{
    public override string Name { get; set; } = "wire-name-document-printer-access";
    public override Color Color { get; set; } = Color.Green;
    public override object StatusKey => AccessWireActionKey.Status;

    public override void ToggleValue(EntityUid owner, bool setting)
    {
        if (EntityManager.TryGetComponent<DocumentPrinterComponent>(owner, out var comp))
            comp.AccessBroken = !setting;

        if (EntityManager.TryGetComponent<AccessReaderComponent>(owner, out var reader))
        {
            EntityManager.System<AccessReaderSystem>().SetActive((owner, reader), setting);
        }

        EntityManager.System<DocumentPrinterSystem>().RefreshUi(owner);
    }

    public override bool GetValue(EntityUid owner)
    {
        return EntityManager.TryGetComponent<DocumentPrinterComponent>(owner, out var comp)
               && !comp.AccessBroken;
    }

    public override StatusLightState? GetLightState(Wire wire)
    {
        return EntityManager.TryGetComponent<DocumentPrinterComponent>(wire.Owner, out var comp) && comp.AccessBroken
            ? StatusLightState.Off
            : StatusLightState.On;
    }
}
