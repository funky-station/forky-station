// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Client.Fluids.UI;
using Content.Client.Items;
using Content.Shared.Fluids;

namespace Content.Client.Fluids;

/// <inheritdoc/>
public sealed class AbsorbentSystem : SharedAbsorbentSystem
{
    public override void Initialize()
    {
        base.Initialize();
        Subs.ItemStatus<AbsorbentComponent>(ent => new AbsorbentItemStatus(ent, EntityManager));
    }
}
