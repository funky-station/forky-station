using Robust.Shared.GameStates;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starfall.Execution;

/// <summary>
/// Optionally add to a gun to override its execution doafter duration.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class GunExecutionComponent : Component
{
    /// <summary>
    /// The default duration of an execution.
    /// </summary>
    public static readonly TimeSpan DefaultExecutionTime = TimeSpan.FromSeconds(4);

    /// <summary>
    /// How long the execution doafter lasts.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DefaultExecutionTime"/>.
    /// </remarks>
    [DataField, AutoNetworkedField]
    public TimeSpan ExecutionTime = DefaultExecutionTime;

    /// <summary>
    /// Damage type used when self-executing with ammunition that has no positive damage type.
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<DamageTypePrototype>? SelfExecutionDamageType;

    /// <summary>
    /// Optional restrictions on who can use the configured self-execution damage.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityWhitelist? SelfExecutionUserWhitelist;
}

/// <summary>
/// Prevents a gun from being used to perform executions.
/// Guns without this component may be used for executions.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class GunExecutionBlacklistComponent : Component
{
}
