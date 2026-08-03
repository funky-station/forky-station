using Content.Shared._ES.Viewcone.Components;
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

    /// <summary>
    ///     Returns the modified viewcone angle for an entity, calculated from the base, taking into account
    ///     equipment & status effects & whatnot
    /// </summary>
    public float GetModifiedViewconeAngle(Entity<ESViewconeComponent?> ent)
    {
        if (!Resolve(ent, ref ent.Comp))
            return 0f;

        var ev = new ESViewconeGetAngleModifierEvent();
        RaiseLocalEvent(ent, ref ev, true);

        // clamps to 0, 360 since this is additive and could easily go over with stacking equipment items and shit
        return Math.Clamp(ent.Comp.BaseConeAngle + ev.GetAngleModifier(), 0f, 360f);
    }
}
