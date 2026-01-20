// SPDX-FileCopyrightText: 2024 LordCarve
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Ame.Components;

[Virtual]
public partial class SharedAmeShieldComponent : Component
{
}

[Serializable, NetSerializable]
public enum AmeShieldVisuals
{
    Core,
    CoreState
}

[Serializable, NetSerializable]
public enum AmeCoreState
{
    Off,
    Weak,
    Strong
}
