// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2022 0x6273
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 pointer-to-null
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.StatusEffect;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.Electrocution;

[AdminCommand(AdminFlags.Fun)]
public sealed class ElectrocuteCommand : LocalizedEntityCommands
{
    [Dependency] private readonly ElectrocutionSystem _electrocution = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;

    public override string Command => "electrocute";

    private static readonly ProtoId<StatusEffectPrototype> ElectrocutionStatusEffect = "Electrocution";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length is < 1 or > 3)
        {
            shell.WriteError(Loc.GetString($"shell-need-between-arguments",
                ("lower", 1),
                ("upper", 3)));
            return;
        }

        if (!NetEntity.TryParse(args[0], out var uidNet) || !EntityManager.TryGetEntity(uidNet, out var uid) || !EntityManager.EntityExists(uid))
        {
            shell.WriteError(Loc.GetString($"shell-could-not-find-entity-with-uid", ("uid", args[0])));
            return;
        }

        if (!_statusEffects.CanApplyEffect(uid.Value, ElectrocutionStatusEffect))
        {
            shell.WriteError(Loc.GetString("cmd-electrocute-entity-cannot-be-electrocuted"));
            return;
        }

        if (args.Length < 2 || !int.TryParse(args[1], out var seconds))
            seconds = 10;

        if (args.Length < 3 || !int.TryParse(args[2], out var damage))
            damage = 10;

        _electrocution.TryDoElectrocution(uid.Value, null, damage, TimeSpan.FromSeconds(seconds), refresh: true, ignoreInsulation: true);
    }
}
