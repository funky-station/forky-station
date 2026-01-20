// SPDX-FileCopyrightText: 2025 chromiumboy
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Ben
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Julian Giebel
// SPDX-FileCopyrightText: 2020 juliangiebel
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Construction.EntitySystems;
using Content.Shared.Tools;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Construction.Components
{
    [RegisterComponent, Access(typeof(AnchorableSystem)), NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class AnchorableComponent : Component
    {
        [DataField]
        public ProtoId<ToolQualityPrototype> Tool { get; private set; } = "Anchoring";

        [DataField, AutoNetworkedField]
        public AnchorableFlags Flags = AnchorableFlags.Anchorable | AnchorableFlags.Unanchorable;

        [DataField]
        [ViewVariables(VVAccess.ReadWrite)]
        public bool Snap { get; private set; } = true;

        /// <summary>
        /// Base delay to use for anchoring.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField]
        public float Delay = 1f;
    }

    [Flags]
    public enum AnchorableFlags : byte
    {
        None = 0,
        Anchorable = 1 << 0,
        Unanchorable = 1 << 1,
    }

    public abstract class BaseAnchoredAttemptEvent : CancellableEntityEventArgs
    {
        public EntityUid User { get; }
        public EntityUid Tool { get; }

        /// <summary>
        ///     Extra delay to add to the do_after.
        ///     Add to this, don't replace it.
        ///     Output parameter.
        /// </summary>
        public float Delay { get; set; } = 0f;

        protected BaseAnchoredAttemptEvent(EntityUid user, EntityUid tool)
        {
            User = user;
            Tool = tool;
        }
    }

    public sealed class AnchorAttemptEvent : BaseAnchoredAttemptEvent
    {
        public AnchorAttemptEvent(EntityUid user, EntityUid tool) : base(user, tool) { }
    }

    public sealed class UnanchorAttemptEvent : BaseAnchoredAttemptEvent
    {
        public UnanchorAttemptEvent(EntityUid user, EntityUid tool) : base(user, tool) { }
    }

    public abstract class BaseAnchoredEvent : EntityEventArgs
    {
        public EntityUid User { get; }
        public EntityUid Tool { get; }

        protected BaseAnchoredEvent(EntityUid user, EntityUid tool)
        {
            User = user;
            Tool = tool;
        }
    }

    /// <summary>
    ///     Raised just before the entity's body type is changed.
    /// </summary>
    public sealed class BeforeAnchoredEvent : BaseAnchoredEvent
    {
        public BeforeAnchoredEvent(EntityUid user, EntityUid tool) : base(user, tool) { }
    }

    /// <summary>
    ///     Raised when an entity with an anchorable component is anchored. Note that you may instead want the more
    ///     general <see cref="AnchorStateChangedEvent"/>. This event has the benefit of having user & tool information,
    ///     as a result of interactions mediated by the <see cref="AnchorableSystem"/>.
    /// </summary>
    public sealed class UserAnchoredEvent : BaseAnchoredEvent
    {
        public UserAnchoredEvent(EntityUid user, EntityUid tool) : base(user, tool) { }
    }

    /// <summary>
    ///     Raised just before the entity's body type is changed.
    /// </summary>
    public sealed class BeforeUnanchoredEvent : BaseAnchoredEvent
    {
        public BeforeUnanchoredEvent(EntityUid user, EntityUid tool) : base(user, tool) { }
    }

    /// <summary>
    ///     Raised when an entity with an anchorable component is unanchored. Note that you will probably also need to
    ///     subscribe to the more general <see cref="AnchorStateChangedEvent"/>, which gets raised BEFORE this one. This
    ///     event has the benefit of having user & tool information, whereas the more general event may be due to
    ///     explosions or grid-destruction or other interactions not mediated by the <see cref="AnchorableSystem"/>.
    /// </summary>
    public sealed class UserUnanchoredEvent : BaseAnchoredEvent
    {
        public UserUnanchoredEvent(EntityUid user, EntityUid tool) : base(user, tool) { }
    }
}
