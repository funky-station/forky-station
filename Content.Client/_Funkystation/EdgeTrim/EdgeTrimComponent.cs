using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Client._FunkyStation.EdgeTrim
{
    /// <summary>
    ///     Makes sprites of other anchored entities adjust based on the trimming keys that they have. IconSmoothing
    ///     if it were good. Allows for different sprites to be used if edge keys are selected.
    /// </summary>
    /// <remarks>
    ///     The system is based on IconSmoothing's corners smoothing mode, converting sprites needs a '-smooth'
    ///     placed after the base but before the number. Navigate to, from the base directory,
    ///     Tools/SS14 Aseprite Templates/EdgeTrimming for more details.
    ///     To use, set <c>base</c> equal to the prefix of the corner states in the sprite base RSI.
    ///     Any objects with the same <c>key</c> or one listed in AdditionalKeys will connect.
    ///     Any objects with a <c>key</c> listed in EdgeKeys will use special edge trimming sprites.
    /// </remarks>
    [RegisterComponent]
    public sealed partial class EdgeTrimComponent : Component
    {
        [ViewVariables(VVAccess.ReadWrite), DataField("enabled")]
        public bool Enabled = true;

        public (EntityUid?, Vector2i)? LastPosition;

        /// <summary>
        ///     We will smooth with other objects with the same key.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite), DataField("smoothKey")]
        public string? SmoothKey { get; private set; }

        /// <summary>
        ///     Additional keys to smooth with.
        /// </summary>
        [DataField("addSmoothKeys")]
        public List<string> AdditionalKeys = new();

        /// <summary>
        ///     Keys to place an edge against
        /// </summary>
        [DataField("edgeKeys")]
        public List<string> EdgeKeys = new();

        /// <summary>
        ///     Prepended to the RSI state.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite), DataField("base")]
        public string StateBase { get; set; } = string.Empty;

        [DataField("shader", customTypeSerializer: typeof(PrototypeIdSerializer<ShaderPrototype>))]
        public string? Shader;

        /// <summary>
        ///     Mode that controls how the icon should be selected.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite), DataField("mode")]
        public EdgeTrimMode Mode = EdgeTrimMode.Default;

        /// <summary>
        ///     Used by <see cref="EdgeTrimSystem"/> to reduce redundant updates.
        /// </summary>
        internal int UpdateGeneration { get; set; }
    }

    /// <summary>
    ///     Controls the mode with which trim is calculated.
    /// </summary>
    [PublicAPI]
    public enum EdgeTrimMode : byte
    {
        /// <summary>
        ///     Each icon is made up of 4 corners, each of which can get a different state depending on
        ///     adjacent entities registered counterclockwise with the corner.
        /// </summary>
        Default,

        /// <summary>
        ///     Where this component contributes to its neighbors being calculated but we do not update its own sprite.
        /// </summary>
        NoSprite,
    }
}
