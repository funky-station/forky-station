// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2024 SpeltIncorrectyl
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Power.Generator;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ActiveGeneratorRevvingComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public TimeSpan CurrentTime = TimeSpan.Zero;
}
