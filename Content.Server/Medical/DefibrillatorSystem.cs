// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2026 slarticodefast
// SPDX-FileCopyrightText: 2025 Ciarán Walsh
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2025 Milon
// SPDX-FileCopyrightText: 2025 Zachary Higgs
// SPDX-FileCopyrightText: 2024 beck-thompson
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2024 Cojoke
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Scribbles0
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Raphael Bertoche
// SPDX-FileCopyrightText: 2023 ShadowCommander
// SPDX-License-Identifier: MIT

using Content.Server.EUI;
using Content.Server.Ghost;
using Content.Shared.Medical;
using Content.Shared.Mind;
using Robust.Shared.Player;

namespace Content.Server.Medical;

public sealed class DefibrillatorSystem : SharedDefibrillatorSystem
{
    [Dependency] private readonly EuiManager _eui = default!;
    [Dependency] private readonly ISharedPlayerManager _player = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    protected override void OpenReturnToBodyEui(Entity<MindComponent> mind, ICommonSession session)
    {
        _eui.OpenEui(new ReturnToBodyEui(mind, _mind, _player), session);
    }
}
