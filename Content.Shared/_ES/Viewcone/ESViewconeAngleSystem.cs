using Content.Shared._ES.Viewcone.Components;
using Content.Shared.Disposal.Unit;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.StatusEffectNew;

namespace Content.Shared._ES.Viewcone;

/// <summary>
///     Public API for getting the actual modified viewcone angle (including equipment etc) rather than just the base angle
/// </summary>
public sealed class ESViewconeAngleSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ESViewconeModifierComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<ESViewconeModifierComponent, ESViewconeGetAngleModifierEvent>(OnAngleModify);
        SubscribeLocalEvent<ESViewconeModifierComponent, InventoryRelayedEvent<ESViewconeGetAngleModifierEvent>>(OnAngleInventoryModify);
        SubscribeLocalEvent<ESViewconeModifierComponent, StatusEffectRelayedEvent<ESViewconeGetAngleModifierEvent>>(OnAngleStatusEffectModify);

        SubscribeLocalEvent<BeingDisposedComponent, ESViewconeGetAngleModifierEvent>(OnBeingDisposedAngle);
    }

    private void OnExamined(Entity<ESViewconeModifierComponent> ent, ref ExaminedEvent args)
    {
        var loc = "es-viewcone-modifier-examine-increase";
        if (ent.Comp.AngleModifier < 0)
            loc = "es-viewcone-modifier-examine-decrease";

        var degrees = (int) MathF.Abs(ent.Comp.AngleModifier);
        args.PushMarkup(Loc.GetString(loc, ("degrees", degrees)));
    }

    private void OnAngleModify(Entity<ESViewconeModifierComponent> ent, ref ESViewconeGetAngleModifierEvent args)
    {
        args.ModifyAngle(ent.Comp.AngleModifier);
    }

    private void OnAngleInventoryModify(Entity<ESViewconeModifierComponent> ent, ref InventoryRelayedEvent<ESViewconeGetAngleModifierEvent> args)
    {
        args.Args.ModifyAngle(ent.Comp.AngleModifier);
    }

    private void OnAngleStatusEffectModify(Entity<ESViewconeModifierComponent> ent, ref StatusEffectRelayedEvent<ESViewconeGetAngleModifierEvent> args)
    {
        args.Args.ModifyAngle(ent.Comp.AngleModifier);
    }

    private void OnBeingDisposedAngle(Entity<BeingDisposedComponent> ent, ref ESViewconeGetAngleModifierEvent args)
    {
        args.ModifyAngle(-360f);
    }

    /// <summary>
    ///     Returns the modified viewcone angle for an entity, calculated from the base, taking into account
    ///     equipment & status effects & whatnot
    /// </summary>
    public float GetModifiedViewconeAngle(Entity<ESViewconeComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return 0f;

        // Funky start
        var viewcone = ent.Comp;

        if (ent.Comp.IsBlind)
            viewcone.DesiredConeAngle = viewcone.BaseConeAngleBlind;
        else
        {
            var ev = new ESViewconeGetAngleModifierEvent();
            RaiseLocalEvent(ent, ref ev, true);
            viewcone.DesiredConeAngle = viewcone.BaseConeAngle + ev.GetAngleModifier();
        }

        // CurrentAngle gets lerped each frame in ViewconeBlindSystem
        return ent.Comp.CurrentConeAngle;
        // Funky end
    }

    // Funky - we need this method to modify the ConeIgnoreRadius depending on if
    // we're blinded or not
    public float GetModifiedConeIgnoreRadius(Entity<ESViewconeComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return 0f;

        if (ent.Comp.IsBlind)
            return ent.Comp.ConeIgnoreRadiusBlind;

        return ent.Comp.ConeIgnoreRadius;
    }
}
