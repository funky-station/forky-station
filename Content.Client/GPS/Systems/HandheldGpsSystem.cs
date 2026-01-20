// SPDX-FileCopyrightText: 2024 ArchRBX
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Content.Shared.GPS.Components;
using Content.Client.GPS.UI;
using Content.Client.Items;

namespace Content.Client.GPS.Systems;

public sealed class HandheldGpsSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        Subs.ItemStatus<HandheldGPSComponent>(ent => new HandheldGpsStatusControl(ent));
    }
}
