// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Shared.Alert;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Shuttles.Components
{
    /// <summary>
    /// Stores what shuttle this entity is currently piloting.
    /// </summary>
    [RegisterComponent]
    [NetworkedComponent]
    public sealed partial class PilotComponent : Component
    {
        [ViewVariables]
        public EntityUid? Console { get; set; }

        /// <summary>
        /// Where we started piloting from to check if we should break from moving too far.
        /// </summary>
        [ViewVariables]
        public EntityCoordinates? Position { get; set; }

        public Vector2 CurTickStrafeMovement = Vector2.Zero;
        public float CurTickRotationMovement;
        public float CurTickBraking;

        public GameTick LastInputTick = GameTick.Zero;
        public ushort LastInputSubTick = 0;

        [ViewVariables]
        public ShuttleButtons HeldButtons = ShuttleButtons.None;

        [DataField]
        public ProtoId<AlertPrototype> PilotingAlert = "PilotingShuttle";

        public override bool SendOnlyToOwner => true;
    }

    public sealed partial class StopPilotingAlertEvent : BaseAlertEvent;
}
