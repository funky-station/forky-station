// SPDX-FileCopyrightText: 2023-2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pspritechologist <81725545+Pspritechologist@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Fildrance <fildrance@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Server.Power.EntitySystems;
using Content.Server.Radio;
using Content.Server.SurveillanceCamera;
using Content.Shared.Emp;

namespace Content.Server.Emp;

public sealed class EmpSystem : SharedEmpSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EmpDisabledComponent, RadioSendAttemptEvent>(OnRadioSendAttempt);
        SubscribeLocalEvent<EmpDisabledComponent, RadioReceiveAttemptEvent>(OnRadioReceiveAttempt);
        SubscribeLocalEvent<EmpDisabledComponent, ApcToggleMainBreakerAttemptEvent>(OnApcToggleMainBreaker);
        SubscribeLocalEvent<EmpDisabledComponent, SurveillanceCameraSetActiveAttemptEvent>(OnCameraSetActive);
    }

    private void OnRadioSendAttempt(EntityUid uid, EmpDisabledComponent component, ref RadioSendAttemptEvent args)
    {
        args.Cancelled = true;
    }

    private void OnRadioReceiveAttempt(EntityUid uid, EmpDisabledComponent component, ref RadioReceiveAttemptEvent args)
    {
        args.Cancelled = true;
    }

    private void OnApcToggleMainBreaker(EntityUid uid, EmpDisabledComponent component, ref ApcToggleMainBreakerAttemptEvent args)
    {
        args.Cancelled = true;
    }

    private void OnCameraSetActive(EntityUid uid, EmpDisabledComponent component, ref SurveillanceCameraSetActiveAttemptEvent args)
    {
        args.Cancelled = true;
    }
}
