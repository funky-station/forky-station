// SPDX-FileCopyrightText: 2025 Ciarán Walsh <github@ciaranwal.sh>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Client.Guidebook.Controls;

/// <summary>
///    Interface for controls which represent a Prototype
///    These can be linked to from a IPrototypeLinkControl
/// </summary>
public interface IPrototypeRepresentationControl
{
    // The prototype that this control represents
    public IPrototype? RepresentedPrototype { get; }
}

/// <summary>
///    Interface for controls which can be clicked to navigate
///    to a specified prototype representation on the same page.
/// </summary>
public interface IPrototypeLinkControl
{
    // This control is a link to the specified prototype
    public IPrototype? LinkedPrototype { get; }

    // Initially the link will not be enabled,
    // the owner can enable the link once there is a valid target
    // for the Prototype link.
    public void EnablePrototypeLink();
}
