// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 TemporalOroboros
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Pspritechologist
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2023 Slava0135
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
