using Content.Shared.Damage.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Damage.Components;

namespace Content.Server._Starfall.Systems;

public sealed partial class SprintStaminaSystem : EntitySystem
{
    [Dependency] private SharedStaminaSystem _stamina = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var enumerator = EntityQueryEnumerator<InputMoverComponent, StaminaComponent>();

        while (enumerator.MoveNext(out var uid, out var mover, out var stam))
        {
            if (!mover.HasDirectionalMovement || !mover.Sprinting)
                continue;

            // _Stardrift: Drain stamina while sprinting using upstream config.
            var drain = stam.SprintDrainPerSecond * frameTime;
            _stamina.TakeStaminaDamage(uid, drain, component: stam, visual: false);
        }
    }
}
