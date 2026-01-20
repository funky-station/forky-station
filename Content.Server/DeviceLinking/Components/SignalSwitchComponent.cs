// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2023 Julian Giebel
// SPDX-FileCopyrightText: 2022 Morber
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 ShadowCommander
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-License-Identifier: MIT

using Content.Server.DeviceLinking.Systems;
using Content.Shared.DeviceLinking;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.DeviceLinking.Components;

/// <summary>
///     Simple switch that will fire ports when toggled on or off. A button is jsut a switch that signals on the
///     same port regardless of its state.
/// </summary>
[RegisterComponent, Access(typeof(SignalSwitchSystem))]
public sealed partial class SignalSwitchComponent : Component
{
    /// <summary>
    ///     The port that gets signaled when the switch turns on.
    /// </summary>
    [DataField("onPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string OnPort = "On";

    /// <summary>
    ///     The port that gets signaled when the switch turns off.
    /// </summary>
    [DataField("offPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string OffPort = "Off";

    /// <summary>
    ///     The port that gets signaled with the switch's current status.
    ///     This is only used if OnPort is different from OffPort, not in the case of a toggle switch.
    /// </summary>
    [DataField("statusPort", customTypeSerializer: typeof(PrototypeIdSerializer<SourcePortPrototype>))]
    public string StatusPort = "Status";

    [DataField("state")]
    public bool State;

    [DataField("clickSound")]
    public SoundSpecifier ClickSound = new SoundPathSpecifier("/Audio/Machines/lightswitch.ogg");
}
