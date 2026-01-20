// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 MishaUnity
// SPDX-FileCopyrightText: 2022 Julian Giebel
// SPDX-License-Identifier: MIT

namespace Content.Server.CartridgeLoader.Cartridges;

[RegisterComponent]
public sealed partial class NotekeeperCartridgeComponent : Component
{
    /// <summary>
    /// The list of notes that got written down
    /// </summary>
    [DataField("notes")]
    public List<string> Notes = new();
}
