using Content.Shared._Funkystation.Cpr;
using Robust.Shared.Player;

namespace Content.Server._Funkystation.Cpr;

public sealed class CprSystem : SharedCprSystem
{
    public override void DoLunge(EntityUid user)
    {
        // raise event for all nearby players
        var filter = Filter.PvsExcept(user, entityManager: Ent);

        RaiseNetworkEvent(new CprLungeEvent(GetNetEntity(user)), filter);
    }
}
