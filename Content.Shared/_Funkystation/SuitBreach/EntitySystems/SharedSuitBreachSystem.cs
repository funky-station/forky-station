using Content.Shared._Funkystation.SuitBreach.Components;
using Content.Shared.Examine;
using Robust.Shared.Random;

namespace Content.Shared._Funkystation.SuitBreach.EntitySystems;

/// <summary>
/// handles the shared severity for suit breaches
/// </summary>
public abstract partial class SharedSuitBreachSystem : EntitySystem
{
    [Dependency] private IRobustRandom _random = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SuitBreachedComponent, ExaminedEvent>(OnSuitExamined);
        SubscribeLocalEvent<SuitSealantCanisterComponent, ExaminedEvent>(OnCanisterExamined);
    }

    /// <summary>
    /// examining a breached suit tells you how bad it is
    /// </summary>
    private void OnSuitExamined(Entity<SuitBreachedComponent> ent, ref ExaminedEvent args)
    {
        var locKey = ent.Comp.Severity switch
        {
            SuitBreachSeverity.Minor => "suit-breach-examine-minor",
            SuitBreachSeverity.Major => "suit-breach-examine-major",
            SuitBreachSeverity.Catastrophic => "suit-breach-examine-catastrophic",
            _ => null,
        };

        if (locKey != null)
            args.PushMarkup(Loc.GetString(locKey));
    }

    /// <summary>
    /// examining a sealant canister shows how many charges it has left
    /// </summary>
    private void OnCanisterExamined(Entity<SuitSealantCanisterComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(ent.Comp.Charges > 0
            ? Loc.GetString("suit-breach-sealant-examine-charges", ("charges", ent.Comp.Charges))
            : Loc.GetString("suit-breach-sealant-examine-empty"));
    }

    /// <summary>
    /// escalates breach severity
    /// </summary>
    protected bool TryEscalate(ref Entity<SuitBreachedComponent?> suit, SuitBreachSeverity incoming)
    {
        if (incoming == SuitBreachSeverity.None)
            return false;

        if (!Resolve(suit, ref suit.Comp, logMissing: false))
        {
            var comp = EnsureComp<SuitBreachedComponent>(suit);
            comp.Severity = incoming;
            comp.LeakAngle = _random.NextAngle();
            Dirty(suit, comp);
            suit.Comp = comp;
            return true;
        }

        if (suit.Comp.Severity == SuitBreachSeverity.Catastrophic)
            return false;

        // if the new breach isn't strictly worse than current, make it worse anyway so that many small hits still accumulate
        var nextSeverity = incoming > suit.Comp.Severity
            ? incoming
            : suit.Comp.Severity + 1;

        suit.Comp.Severity = nextSeverity;
        Dirty(suit, suit.Comp);
        return true;
    }

    /// <summary>
    /// regresses breach severity by one tier
    /// </summary>
    protected SuitBreachSeverity Regress(Entity<SuitBreachedComponent> suit)
    {
        var next = suit.Comp.Severity switch
        {
            SuitBreachSeverity.Catastrophic => SuitBreachSeverity.Major,
            SuitBreachSeverity.Major => SuitBreachSeverity.Minor,
            _ => SuitBreachSeverity.None,
        };

        if (next == SuitBreachSeverity.None)
        {
            RemComp<SuitBreachedComponent>(suit);
            return SuitBreachSeverity.None;
        }

        suit.Comp.Severity = next;
        Dirty(suit, suit.Comp);
        return next;
    }
}
