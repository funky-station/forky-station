// SPDX-FileCopyrightText: 2021-2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022-2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023, 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2025 Nikovnik <116634167+nkokic@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 āda <ss.adasts@gmail.com>
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Qerd <73325910+BigfootBravo@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared.Body.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(StomachSystem))]
public sealed partial class StomachComponent : Component
{
    /// <summary>
    /// The solution inside of this stomach
    /// </summary>
    [ViewVariables]
    public Entity<SolutionComponent>? Solution;

    /// <summary>
    ///     A whitelist for what special-digestible-required foods this stomach is capable of eating.
    /// </summary>
    [DataField]
    public EntityWhitelist? SpecialDigestible = null;

    /// <summary>
    /// Controls whitelist behavior. If true, this stomach can digest <i>only</i> food that passes the whitelist. If false, it can digest normal food <i>and</i> any food that passes the whitelist.
    /// </summary>
    [DataField]
    public bool IsSpecialDigestibleExclusive = true;
}
