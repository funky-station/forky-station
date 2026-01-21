// SPDX-FileCopyrightText: 2020-2021, 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 F77F <66768086+F77F@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021-2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021-2022 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Galactic Chimp <63882831+GalacticChimp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components
{
    /// <summary>
    /// Ignores gravity entirely.
    /// </summary>
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class MovementIgnoreGravityComponent : Component
    {
        /// <summary>
        /// Whether gravity is on or off for this object. This will always override the current Gravity State.
        /// </summary>
        [DataField, AutoNetworkedField]
        public bool Weightless;
    }
}
