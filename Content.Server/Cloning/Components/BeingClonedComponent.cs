// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-License-Identifier: MIT

using Content.Shared.Mind;

namespace Content.Server.Cloning.Components
{
    [RegisterComponent]
    public sealed partial class BeingClonedComponent : Component
    {
        [ViewVariables]
        public MindComponent? Mind = default;

        [ViewVariables]
        public EntityUid Parent;
    }
}
