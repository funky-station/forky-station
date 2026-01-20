// SPDX-FileCopyrightText: 2025 SlamBamActionman
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2022 rolfero
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 Ripmorld
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Movement.Components;
using Content.Server.Movement.Systems;
using Content.Shared.Camera;
using Content.Shared.Hands;
using Content.Shared.Movement.Components;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;

namespace Content.Server.Wieldable;

public sealed class WieldableSystem : SharedWieldableSystem
{
    [Dependency] private readonly ContentEyeSystem _eye = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CursorOffsetRequiresWieldComponent, ItemUnwieldedEvent>(OnEyeOffsetUnwielded);
        SubscribeLocalEvent<CursorOffsetRequiresWieldComponent, ItemWieldedEvent>(OnEyeOffsetWielded);
        SubscribeLocalEvent<CursorOffsetRequiresWieldComponent, HeldRelayedEvent<GetEyePvsScaleRelayedEvent>>(OnGetEyePvsScale);
    }

    private void OnEyeOffsetUnwielded(Entity<CursorOffsetRequiresWieldComponent> entity, ref ItemUnwieldedEvent args)
    {
        _eye.UpdatePvsScale(args.User);
    }

    private void OnEyeOffsetWielded(Entity<CursorOffsetRequiresWieldComponent> entity, ref ItemWieldedEvent args)
    {
        _eye.UpdatePvsScale(args.User);
    }

    private void OnGetEyePvsScale(Entity<CursorOffsetRequiresWieldComponent> entity,
        ref HeldRelayedEvent<GetEyePvsScaleRelayedEvent> args)
    {
        if (!TryComp(entity, out EyeCursorOffsetComponent? eyeCursorOffset) || !TryComp(entity.Owner, out WieldableComponent? wieldableComp))
            return;

        if (!wieldableComp.Wielded)
            return;

        args.Args.Scale += eyeCursorOffset.PvsIncrease;
    }
}
