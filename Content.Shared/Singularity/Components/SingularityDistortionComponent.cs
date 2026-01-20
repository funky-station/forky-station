// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Shared.Singularity.EntitySystems;
using Robust.Shared.GameStates;

namespace Content.Shared.Singularity.Components
{
    [RegisterComponent, NetworkedComponent]
    [AutoGenerateComponentState]
    [Access(typeof(SharedSingularitySystem))]
    public sealed partial class SingularityDistortionComponent : Component
    {
        [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
        public float Intensity = 31.25f;

        [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
        public float FalloffPower = MathF.Sqrt(2f);
    }
}
