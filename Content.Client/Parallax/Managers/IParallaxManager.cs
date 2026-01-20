// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 20kdc
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using System.Numerics;
using System.Threading.Tasks;
using Robust.Shared.Maths;

namespace Content.Client.Parallax.Managers;

public interface IParallaxManager
{
    /// <summary>
    /// All WorldHomePosition values are offset by this.
    /// </summary>
    Vector2 ParallaxAnchor { get; set; }

    bool IsLoaded(string name);

    /// <summary>
    /// The layers of the selected parallax.
    /// </summary>
    ParallaxLayerPrepared[] GetParallaxLayers(string name);

    /// <summary>
    /// Loads in the default parallax to use.
    /// Do not call until prototype manager is available.
    /// </summary>
    void LoadDefaultParallax();

    Task LoadParallaxByName(string name);

    void UnloadParallax(string name);
}

