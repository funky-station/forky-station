// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Peter Wedder
// SPDX-FileCopyrightText: 2020 Tyler Young
// SPDX-FileCopyrightText: 2020 Clyybber
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2020 Jackson Lewis
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Instruments;
using Robust.Client.Audio.Midi;
using Robust.Shared.Audio.Midi;

namespace Content.Client.Instruments;

[RegisterComponent]
public sealed partial class InstrumentComponent : SharedInstrumentComponent
{
    public event Action? OnMidiPlaybackEnded;

    [ViewVariables]
    public IMidiRenderer? Renderer;

    [ViewVariables]
    public uint SequenceDelay;

    [ViewVariables]
    public uint SequenceStartTick;

    [ViewVariables]
    public TimeSpan LastMeasured = TimeSpan.MinValue;

    [ViewVariables]
    public int SentWithinASec;

    /// <summary>
    ///     A queue of MidiEvents to be sent to the server.
    /// </summary>
    [ViewVariables]
    public readonly List<RobustMidiEvent> MidiEventBuffer = new();

    /// <summary>
    ///     Whether a midi song will loop or not.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool LoopMidi { get; set; } = false;

    /// <summary>
    ///     Whether this instrument is handheld or not.
    /// </summary>
    [DataField("handheld")]
    public bool Handheld { get; set; } // TODO: Replace this by simply checking if the entity has an ItemComponent.

    /// <summary>
    ///     Whether there's a midi song being played or not.
    /// </summary>
    [ViewVariables]
    public bool IsMidiOpen => Renderer?.Status == MidiRendererStatus.File;

    /// <summary>
    ///     Whether the midi renderer is listening for midi input or not.
    /// </summary>
    [ViewVariables]
    public bool IsInputOpen => Renderer?.Status == MidiRendererStatus.Input;

    /// <summary>
    ///     Whether the midi renderer is alive or not.
    /// </summary>
    [ViewVariables]
    public bool IsRendererAlive => Renderer != null;

    [ViewVariables]
    public int PlayerTotalTick => Renderer?.PlayerTotalTick ?? 0;

    [ViewVariables]
    public int PlayerTick => Renderer?.PlayerTick ?? 0;

    public void PlaybackEndedInvoke() => OnMidiPlaybackEnded?.Invoke();
}
