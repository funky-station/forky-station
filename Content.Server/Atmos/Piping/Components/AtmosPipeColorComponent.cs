// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2025 IgorAnt028
// SPDX-FileCopyrightText: 2024 chromiumboy
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.Piping.EntitySystems;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Piping.Components;

[RegisterComponent]
public sealed partial class AtmosPipeColorComponent : Component
{
    [DataField]
    public Color Color { get; set; } = Color.White;

    [ViewVariables(VVAccess.ReadWrite), UsedImplicitly]
    public Color ColorVV
    {
        get => Color;
        set => IoCManager.Resolve<IEntityManager>().System<AtmosPipeColorSystem>().SetColor(Owner, this, value);
    }
}

[ByRefEvent]
public record struct AtmosPipeColorChangedEvent(Color color)
{
    public Color Color = color;
}
