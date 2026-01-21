// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2025 ArtisticRoomba <145879011+ArtisticRoomba@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 IgorAnt028 <118114530+IgorAnt028@users.noreply.github.com>
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
