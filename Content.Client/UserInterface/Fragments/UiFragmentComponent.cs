// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-License-Identifier: MIT

namespace Content.Client.UserInterface.Fragments;

/// <summary>
/// The component used for defining a ui fragment to attach to an entity
/// </summary>
/// <remarks>
/// This is used primarily for PDA cartridges.
/// </remarks>
/// <seealso cref="UIFragment"/>
[RegisterComponent]
public sealed partial class UIFragmentComponent : Component
{
    [DataField("ui", true)]
    public UIFragment? Ui;
}
