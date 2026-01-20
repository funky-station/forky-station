// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Client.UserInterface.Controls;

namespace Content.Client.Viewport
{
    /// <summary>
    ///     Client state that has a main viewport.
    /// </summary>
    /// <remarks>
    ///     Used for taking no-UI screenshots (including things like flash overlay).
    /// </remarks>
    public interface IMainViewportState
    {
        public MainViewport Viewport { get; }
    }
}
