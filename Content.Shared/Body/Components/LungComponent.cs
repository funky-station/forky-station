// SPDX-FileCopyrightText: 2021-2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022, 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 thetuerk <46725294+ThanosDeGraf@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Systems;
using Content.Shared.Alert;
using Content.Shared.Atmos;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Body.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(LungSystem))]
public sealed partial class LungComponent : Component
{
    [DataField]
    [Access(typeof(LungSystem), Other = AccessPermissions.ReadExecute)] // FIXME Friends
    public GasMixture Air = new()
    {
        Volume = 6,
        Temperature = Atmospherics.NormalBodyTemperature
    };

    /// <summary>
    /// The name/key of the solution on this entity which these lungs act on.
    /// </summary>
    [DataField]
    public string SolutionName = "Lung";

    /// <summary>
    /// The solution on this entity that these lungs act on.
    /// </summary>
    [ViewVariables]
    public Entity<SolutionComponent>? Solution = null;

    /// <summary>
    /// The type of gas this lung needs. Used only for the breathing alerts, not actual metabolism.
    /// </summary>
    [DataField]
    public ProtoId<AlertPrototype> Alert = "LowOxygen";
}
