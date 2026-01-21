// SPDX-FileCopyrightText: 2020 Exp <theexp111@gmail.com>
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Doors
{
    [Serializable, NetSerializable]
    public enum AirlockWireStatus
    {
        PowerIndicator,
        BoltIndicator,
        BoltLightIndicator,
        AiControlIndicator,
        AiVisionIndicator,
        TimingIndicator,
        SafetyIndicator,
    }
}
