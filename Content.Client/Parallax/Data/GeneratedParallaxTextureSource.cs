// SPDX-FileCopyrightText: 2022-2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022-2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 20kdc <asdd2808@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Content.Client.Parallax.Managers;
using Robust.Client.Graphics;
using Robust.Shared.Utility;

namespace Content.Client.Parallax.Data;

[UsedImplicitly]
[DataDefinition]
public sealed partial class GeneratedParallaxTextureSource : IParallaxTextureSource
{
    /// <summary>
    /// Parallax config path (the TOML file).
    /// In client resources.
    /// </summary>
    [DataField("configPath")]
    public ResPath ParallaxConfigPath { get; private set; } = new("/parallax_config.toml");

    /// <summary>
    /// ID for debugging, caching, and so forth.
    /// The empty string here is reserved for the original parallax.
    /// It is required to provide a unique ID for any unique config contents.
    /// </summary>
    [DataField("id")]
    public string Identifier { get; private set; } = "other";

    async Task<Texture> IParallaxTextureSource.GenerateTexture(CancellationToken cancel)
    {
        var cache = IoCManager.Resolve<GeneratedParallaxCache>();
        return await cache.Load(Identifier, ParallaxConfigPath, cancel);
    }

    void IParallaxTextureSource.Unload(IDependencyCollection dependencies)
    {
        var cache = dependencies.Resolve<GeneratedParallaxCache>();
        cache.Unload(Identifier);
    }
}

