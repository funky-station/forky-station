using Content.Shared._Funkystation.SM.Components;
using Content.Shared._Funkystation.SM;
using Content.Shared.UserInterface;
using Robust.Server.GameObjects;

namespace Content.Server._Funkystation.SM.EntitySystems;

/// <summary>
/// Supermatter monitoring console: picks an active crystal on the same grid and pushes read-only BUI state.
/// </summary>
public sealed class SupermatterConsoleSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    private float _updateAccumulator;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SupermatterConsoleComponent, BeforeActivatableUIOpenEvent>(OnBeforeUiOpen);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _updateAccumulator += frameTime;
        if (_updateAccumulator < 0.5f)
            return;

        _updateAccumulator = 0f;

        var query = EntityQueryEnumerator<SupermatterConsoleComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out _, out _))
        {
            if (!_ui.IsUiOpen(uid, SupermatterConsoleUiKey.Key))
                continue;
            UpdateBuiState(uid);
        }
    }

    private void OnBeforeUiOpen(EntityUid uid, SupermatterConsoleComponent _, BeforeActivatableUIOpenEvent args)
    {
        UpdateBuiState(uid);
    }

    /// <summary>
    /// Active crystal: same grid as console, not a shard, not yet delaminated. Tie-break: highest <see cref="SharedSupermatterComponent.Power"/>.
    /// </summary>
    public EntityUid? FindActiveSupermatterOnGrid(EntityUid consoleUid, TransformComponent consoleXform)
    {
        var gridUid = consoleXform.GridUid;
        if (gridUid is not { } grid)
            return null;

        EntityUid? best = null;
        var bestPower = float.NegativeInfinity;

        var q = EntityQueryEnumerator<SharedSupermatterComponent, TransformComponent>();
        while (q.MoveNext(out var smUid, out var sm, out var smXform))
        {
            if (smXform.GridUid != grid)
                continue;

            if (HasComp<SupermatterShardComponent>(smUid))
                continue;

            if (sm.Delaminated)
                continue;

            if (sm.Power > bestPower)
            {
                bestPower = sm.Power;
                best = smUid;
            }
        }

        return best;
    }

    private void UpdateBuiState(EntityUid consoleUid)
    {
        if (!TryComp<TransformComponent>(consoleUid, out var xform))
            return;

        var smUid = FindActiveSupermatterOnGrid(consoleUid, xform);
        if (smUid is not { } crystal || !TryComp<SharedSupermatterComponent>(crystal, out var sm))
        {
            _ui.SetUiState(consoleUid, SupermatterConsoleUiKey.Key,
                new SupermatterConsoleBoundUserInterfaceState(false, default, 0, 0, 0, 0, false, 0, 0, 0));
            return;
        }

        var pct = SupermatterSystem.GetIntegrityPercent(sm.Integrity, sm.MaxIntegrity);
        var net = GetNetEntity(crystal);
        var smSys = EntityManager.System<SupermatterSystem>();
        var predicted = smSys.ChooseDelamType(crystal, sm);
        _ui.SetUiState(consoleUid, SupermatterConsoleUiKey.Key,
            new SupermatterConsoleBoundUserInterfaceState(
                true,
                net,
                pct,
                sm.Integrity,
                sm.MaxIntegrity,
                sm.Power,
                sm.Delamming,
                sm.AbsorbedGas.TotalMoles,
                (byte)predicted,
                sm.Delamming ? sm.DelamCountdown : 0f));
    }
}
