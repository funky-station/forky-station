using Content.Shared._Funkystation.Clothing.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Inventory;

namespace Content.Shared._Funkystation.Clothing.Systems;

public sealed class ClothingStaminaRegenModifierSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ClothingStaminaRegenModifierComponent, BeforeStaminaDamageEvent>(OnGetRegen);
        SubscribeLocalEvent<ClothingStaminaRegenModifierComponent, InventoryRelayedEvent<BeforeStaminaDamageEvent>>(OnRelayedRegen);
    }

    private void OnGetRegen(Entity<ClothingStaminaRegenModifierComponent> ent, ref BeforeStaminaDamageEvent args)
    {
        if (args.Value < 0f)
            args.Value *= ent.Comp.RegenCoefficient;
    }

    private void OnRelayedRegen(Entity<ClothingStaminaRegenModifierComponent> ent, ref InventoryRelayedEvent<BeforeStaminaDamageEvent> args)
    {
        OnGetRegen(ent, ref args.Args);
    }
}
