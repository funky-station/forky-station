// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Server.DeviceNetwork.Systems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.NodeContainer;

namespace Content.Server.DeviceNetwork.Components
{
    [RegisterComponent]
    [Access(typeof(ApcNetworkSystem))]
    [ComponentProtoName("ApcNetworkConnection")]
    public sealed partial class ApcNetworkComponent : Component
    {
        /// <summary>
        /// The node Group the ApcNetworkConnection is connected to
        /// </summary>
        [ViewVariables] public Node? ConnectedNode;
    }
}
