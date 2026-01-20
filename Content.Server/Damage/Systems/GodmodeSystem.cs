// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Dawid Bla
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos.Components;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;

namespace Content.Server.Damage.Systems;

public sealed class GodmodeSystem : SharedGodmodeSystem
{
    public override void EnableGodmode(EntityUid uid, GodmodeComponent? godmode = null)
    {
        godmode ??= EnsureComp<GodmodeComponent>(uid);

        base.EnableGodmode(uid, godmode);

        if (TryComp<MovedByPressureComponent>(uid, out var moved))
        {
            godmode.WasMovedByPressure = moved.Enabled;
            moved.Enabled = false;
        }
    }

    public override void DisableGodmode(EntityUid uid, GodmodeComponent? godmode = null)
    {
    	if (!Resolve(uid, ref godmode, false))
    	    return;

        base.DisableGodmode(uid, godmode);

        if (godmode.Deleted)
            return;

        if (TryComp<MovedByPressureComponent>(uid, out var moved))
        {
            moved.Enabled = godmode.WasMovedByPressure;
        }
    }
}
