using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Content.Shared.Atmos;

namespace Content.Shared.Fluids.Components
{
    /// <summary>
    /// Puddle on a floor
    /// </summary>
    [RegisterComponent, NetworkedComponent, Access(typeof(SharedPuddleSystem))]
    public sealed partial class PuddleComponent : Component
    {
        [DataField]
        public SoundSpecifier SpillSound = new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg");

        [DataField]
        public FixedPoint2 OverflowVolume = FixedPoint2.New(50);

        [DataField("solution")] public string SolutionName = "puddle";

        /// <summary>
        /// Default minimum speed someone must be moving to slip for all reagents.
        /// </summary>
        [DataField]
        public float DefaultSlippery = 5.5f;

        [ViewVariables]
        public Entity<SolutionComponent>? Solution;

        // amount before someone can drown in a puddle, for some reason the U required to is double this amount.
        [DataField]
        public FixedPoint2 DrownU = FixedPoint2.New(100);

        [DataField]
        public bool AffectsMovement = true;

        [DataField]
        public bool AffectsSound = true;
    }
}
