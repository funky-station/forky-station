using Content.Shared._ES.Viewcone.Components;
using Content.Shared._Funkystation.Viewcone;
using Robust.Client.Player;

namespace Content.Client._Funkystation.Overlays.Viewcone;

/// <summary>
/// Handles viewcone blindness client prediction
/// Also see <see cref="ESViewconeComponent"/>
/// </summary>
public sealed partial class ViewconeBlindSystem : EntitySystem
{
    [Dependency] private IPlayerManager _player = default!;

    private const float LerpHalfLife = 0.1f;

    // We use this to lerp from CurrentConeAngle to DesiredConeAngle
    // so we get a smoother ConeAngle transition
    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);

        var enumerator = AllEntityQuery<ESViewconeComponent>();
        while (enumerator.MoveNext(out var _, out var viewcone))
        {
            if (viewcone.DesiredConeAngle.Equals(viewcone.CurrentConeAngle))
                continue;

            // framerate-independent lerp
            // https://twitter.com/FreyaHolmer/status/1757836988495847568
            viewcone.CurrentConeAngle = MathHelper.Lerp(viewcone.CurrentConeAngle, viewcone.DesiredConeAngle, 1f - MathF.Pow(2f, -(frameTime / LerpHalfLife)));
        }
    }

    /// <summary>
    /// Client handler for <see cref="ViewconeStorageClosedEvent"/>.
    /// Predicts the state of <see cref="ESViewconeComponent.IsBlind"/>.
    /// The server state gets reconciled with <see cref="OnViewconeStorageClosedAfterEvent"/>.
    /// </summary>
    [SubscribeLocalEvent]
    private void OnViewconeStorageClosedEvent(Entity<ViewconeBlindnessComponent> ent, ref ViewconeStorageClosedEvent args)
    {
        if (ent != _player.LocalEntity)
            return;

        if (!TryComp<ESViewconeComponent>(ent, out var comp))
            return;

        comp.IsBlind = true;
    }

    /// <summary>
    /// Client handler for <see cref="ViewconeStorageOpenedEvent"/>.
    /// Predicts the state of <see cref="ESViewconeComponent.IsBlind"/>.
    /// The server state gets reconciled with <see cref="OnViewconeStorageOpenedAfterEvent"/>.
    /// </summary>
    [SubscribeLocalEvent]
    private void OnViewconeStorageOpenedEvent(Entity<ViewconeBlindnessComponent> ent, ref ViewconeStorageOpenedEvent args)
    {
        if (ent != _player.LocalEntity)
            return;

        if (!TryComp<ESViewconeComponent>(ent, out var comp))
            return;

        comp.IsBlind = false;
    }

    [SubscribeLocalEvent]
    private void OnAfterState(Entity<ViewconeBlindnessComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (!TryComp<ESViewconeComponent>(ent, out var comp))
            return;

        // Reconcile with the server state
        comp.IsBlind = ent.Comp.IsBlind;
    }
}
