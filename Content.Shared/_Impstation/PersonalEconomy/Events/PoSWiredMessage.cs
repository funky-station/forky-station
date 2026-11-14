using Content.Shared._Impstation.PersonalEconomy.Components;

namespace Content.Shared._Impstation.PersonalEconomy.Events;

// broadcast when a POS sale resolves, so the server can pulse the device-link port (and anything else can react)
public sealed class PoSWiredMessage(Entity<PosSystemComponent> ent, bool success) : EntityEventArgs
{
    public Entity<PosSystemComponent> Ent = ent;
    public bool Success = success;
}
