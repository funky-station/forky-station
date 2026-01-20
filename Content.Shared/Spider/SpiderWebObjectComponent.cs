// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Jackrost
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Spider;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedSpiderSystem))]
public sealed partial class SpiderWebObjectComponent : Component
{
}
