using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.SM.Components;

/// <summary>
/// Marks an entity as safe to touch or use items against the supermatter (testing / special cases).
/// </summary>
[NetworkedComponent, RegisterComponent]
public sealed partial class SupermatterImmuneComponent : Component;
