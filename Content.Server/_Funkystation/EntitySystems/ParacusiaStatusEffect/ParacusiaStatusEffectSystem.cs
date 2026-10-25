using Content.Server.Traits.Assorted;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Prototypes;

namespace Content.Server._Funkystation.EntitySystems.ParacusiaStatusEffect;

/// <summary>
/// Gives the affected entity paracusia as a status effect
/// </summary>
public sealed partial class ParacusiaStatusEffectSystem : EntitySystem
{
    public static readonly EntProtoId ParacusiaStatusEffect = "StatusEffectParacusia";

    [Dependency] private ParacusiaSystem _paracusia = default!;

    [SubscribeLocalEvent]
    private void OnParacusiaEffectApplied(Entity<ParacusiaStatusEffectComponent> ent, ref StatusEffectAppliedEvent args)
    {
        _paracusia.OnComponentStartup(args.Target);
    }

    [SubscribeLocalEvent]
    private void OnParacusiaEffectRemoved(Entity<ParacusiaStatusEffectComponent> ent, ref StatusEffectRemovedEvent args)
    {
        if (ent.Comp.RemovedOnEffectEnd == false)
        {
            return;
        }

        _paracusia.OnPlayerDetach(args.Target);
    }
}
