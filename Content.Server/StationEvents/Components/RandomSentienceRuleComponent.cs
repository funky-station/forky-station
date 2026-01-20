// SPDX-FileCopyrightText: 2024 Psychpsyo
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Server.StationEvents.Events;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(RandomSentienceRule))]
public sealed partial class RandomSentienceRuleComponent : Component
{
    [DataField]
    public int MinSentiences = 1;

    [DataField]
    public int MaxSentiences = 1;
}
