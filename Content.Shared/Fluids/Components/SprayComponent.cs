// SPDX-FileCopyrightText: 2026 chaisftw
// SPDX-FileCopyrightText: 2025 beck-thompson
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 StStevens
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 py01
// SPDX-FileCopyrightText: 2020 DmitriyRubetskoy
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 nuke
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-License-Identifier: MIT

using Content.Shared.FixedPoint;
using Content.Shared.Fluids.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Fluids.Components;

[RegisterComponent]
[Access(typeof(SharedSpraySystem))]
public sealed partial class SprayComponent : Component
{
    [DataField]
    public string Solution = "spray";

    [DataField]
    public FixedPoint2 TransferAmount = 10;

    [DataField]
    public float SprayDistance = 3.5f;

    [DataField]
    public float SprayVelocity = 3.5f;

    [DataField]
    public EntProtoId SprayedPrototype = "Vapor";

    [DataField]
    public int VaporAmount = 1;

    [DataField]
    public float VaporSpread = 90f;

    /// <summary>
    /// How much the player is pushed back for each spray.
    /// </summary>
    [DataField]
    public float PushbackAmount = 5f;

    [DataField(required: true)]
    [Access(typeof(SharedSpraySystem), Other = AccessPermissions.ReadExecute)] // FIXME Friends
    public SoundSpecifier SpraySound { get; private set; } = default!;

    [DataField]
    public LocId SprayEmptyPopupMessage = "spray-component-is-empty-message";
}
