using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Cpr;

/// <summary>
/// entity with this component can have CPR performed on them
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CprComponent : Component
{
    /// <summary>
    /// bonus heal when cpr succeeds
    /// </summary>
    [DataField, AutoNetworkedField]
    public DamageSpecifier BonusHeal;

    [DataField, AutoNetworkedField]
    public SoundSpecifier? Sound;

    [DataField, AutoNetworkedField]
    public EntityUid? LastCaretaker;

    [DataField, AutoNetworkedField]
    public TimeSpan LastTimeGivenCare = TimeSpan.Zero;

    // rib cracking roll only once per crit
    [DataField, AutoNetworkedField]
    public bool HasCrackedRibsThisCrit;

    [DataField, AutoNetworkedField]
    public int PumpsThisCrit;
}
