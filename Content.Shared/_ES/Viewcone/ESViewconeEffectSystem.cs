using Content.Shared._ES.Viewcone.Components;
using Content.Shared._Funkystation.CCVar; // Funky
using JetBrains.Annotations;
using Robust.Shared.Configuration; // Funky
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;

namespace Content.Shared._ES.Viewcone;

/// <summary>
///     API for spawning viewcone effects and making sure source gets set correctly +
///     it spawns in the correct pos and shit
/// </summary>
[PublicAPI]
public sealed partial class ESViewconeEffectSystem : EntitySystem
{
    [Dependency] private INetManager _net = default!;
    [Dependency] private SharedTransformSystem _xform = default!;
    [Dependency] private IConfigurationManager _cfg = default!; // Funky change

    /// <summary>
    ///     Spawns the given effect entity at the player source, and sets relevant variables
    /// </summary>
    /// <param name="source">The player that originated the effect, or the entity to spawn next to if a relevant player doesn't exist</param>
    /// <param name="effect">The prototype ID of an effect entity to spawn (see viewcone_effects.yml)</param>
    /// <param name="angleOverride">The local rotation to set the effect to, instead of the parent rotation.</param>
    public void SpawnEffect(EntityUid source, EntProtoId effect, Angle? angleOverride = null)
    {
        // Do not spawn these clientside at all
        // Server should always handle these, since they shouldn't really be originating from entities that
        // you interact with anyway
        if (_net.IsClient)
            return;

        // Funky
        if (!_cfg.GetCVar(ViewconeCCVars.ViewconeEnabled))
            return;

        var ent = SpawnNextToOrDrop(effect, source);
        var viewconeEffect = EnsureComp<ESViewconeOccludableComponent>(ent);
        viewconeEffect.Inverted = true;
        viewconeEffect.Source = source;
        Dirty(ent, viewconeEffect);

        // set rotation
        _xform.SetLocalRotation(ent, angleOverride ?? Transform(source).LocalRotation);

        // also ensure this in case somehow something without it gets here.
        EnsureComp<TimedDespawnComponent>(ent);
    }
}
