// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 ShadowCommander
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 nikthechampiongr
// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2023 Julian Giebel
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2023 Morb
// SPDX-FileCopyrightText: 2022 0x6273
// SPDX-FileCopyrightText: 2022 Visne
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Kara D
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Server.Power.Components;
using Content.Server.Wires;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Interaction;
using Content.Shared.Power;
using Content.Shared.Wires;
using Robust.Shared.Player;

namespace Content.Server.Doors.Systems;

public sealed class AirlockSystem : SharedAirlockSystem
{
    [Dependency] private readonly WiresSystem _wiresSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AirlockComponent, SignalReceivedEvent>(OnSignalReceived);

        SubscribeLocalEvent<AirlockComponent, PowerChangedEvent>(OnPowerChanged);
        SubscribeLocalEvent<AirlockComponent, ActivateInWorldEvent>(OnActivate, before: new[] { typeof(DoorSystem) });
    }

    private void OnSignalReceived(EntityUid uid, AirlockComponent component, ref SignalReceivedEvent args)
    {
        if (args.Port == component.AutoClosePort && component.AutoClose)
        {
            component.AutoClose = false;
            Dirty(uid, component);
        }
    }

    private void OnPowerChanged(EntityUid uid, AirlockComponent component, ref PowerChangedEvent args)
    {
        component.Powered = args.Powered;
        Dirty(uid, component);

        if (!TryComp(uid, out DoorComponent? door))
            return;

        if (!args.Powered)
        {
            // stop any scheduled auto-closing
            if (door.State == DoorState.Open)
                DoorSystem.SetNextStateChange(uid, null);
        }
        else
        {
            UpdateAutoClose(uid, door: door);
        }
    }

    private void OnActivate(EntityUid uid, AirlockComponent component, ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        if (TryComp<WiresPanelComponent>(uid, out var panel) &&
            panel.Open &&
            TryComp<ActorComponent>(args.User, out var actor))
        {
            if (TryComp<WiresPanelSecurityComponent>(uid, out var wiresPanelSecurity) &&
                !wiresPanelSecurity.WiresAccessible)
                return;

            _wiresSystem.OpenUserInterface(uid, actor.PlayerSession);
            args.Handled = true;
            return;
        }

        if (component.KeepOpenIfClicked && component.AutoClose)
        {
            // Disable auto close
            component.AutoClose = false;
            Dirty(uid, component);
        }
    }
}
