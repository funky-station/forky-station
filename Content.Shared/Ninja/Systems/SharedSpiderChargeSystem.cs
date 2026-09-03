using Content.Shared.Inventory.Events; //funky
using Content.Shared.Ninja.Components; //funky

namespace Content.Shared.Ninja.Systems;

//funky
/// <summary>
/// Sticking triggering and exploding are all in server so this is just for recalling.
/// </summary>
public abstract partial class SharedSpiderChargeSystem : EntitySystem
{
    [Dependency] private SharedSpaceNinjaSystem _ninja = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpiderChargeComponent, GotEquippedEvent>(OnEquipped);
    }
    private void OnEquipped(Entity<SpiderChargeComponent> ent, ref GotEquippedEvent args)
    {
        _ninja.BindSpiderCharge(args.EquipTarget, ent);
    }
}
