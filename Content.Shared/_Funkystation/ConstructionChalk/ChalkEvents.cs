using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.ConstructionChalk;

// place a chalk mark for the chosen recipe at a location
[Serializable, NetSerializable]
public sealed class ChalkPlaceMarkEvent(
    NetEntity chalk,
    NetCoordinates coordinates,
    string constructionPrototype,
    Angle rotation)
    : EntityEventArgs
{
    public NetEntity Chalk = chalk;
    public NetCoordinates Coordinates = coordinates;
    public string ConstructionPrototype = constructionPrototype;
    public Angle Rotation = rotation;
}
