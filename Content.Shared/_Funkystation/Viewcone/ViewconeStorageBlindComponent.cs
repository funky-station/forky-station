using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Viewcone;

// marker for when an entity is in storage like crates, and therefore MUST BE BLINDED!!!
[RegisterComponent, NetworkedComponent]
public sealed partial class ViewconeStorageBlindComponent : Component;
