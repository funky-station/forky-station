// SPDX-FileCopyrightText: 2025 jhrushbe <capnmerry@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.DeviceLinking;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._FarHorizons.Power.Generation.FissionGenerator;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class GasTurbineMonitorComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public NetEntity? turbine;

    [DataField]
    public ProtoId<SinkPortPrototype> LinkingPort = "GasTurbineDataReceiver";
}