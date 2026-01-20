// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-License-Identifier: MIT

using Content.Shared.Interaction.Components;

namespace Content.Shared.Interaction;

public abstract partial class SharedInteractionSystem
{
    public void SetRelay(EntityUid uid, EntityUid? relayEntity, InteractionRelayComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        component.RelayEntity = relayEntity;
        Dirty(uid, component);
    }
}
