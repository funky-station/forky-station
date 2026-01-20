// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

using Content.Shared.CCVar;

namespace Content.Server.CrewManifest;

[RegisterComponent]
public sealed partial class CrewManifestViewerComponent : Component
{
    /// <summary>
    ///     If this manifest viewer is unsecure or not. If it is,
    ///     <see cref="CCVars.CrewManifestUnsecure"/> being false will
    ///     not allow this entity to be processed by CrewManifestSystem.
    /// </summary>
    [DataField("unsecure")] public bool Unsecure;

    /// <summary>
    /// The owner interface of this crew manifest viewer. When it closes, so too will an opened crew manifest.
    /// </summary>
    [DataField(required: true)]
    public Enum OwnerKey { get; private set; } = default!;
}
