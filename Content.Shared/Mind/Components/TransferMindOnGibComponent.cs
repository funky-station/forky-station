// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-License-Identifier: MIT

using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared.Mind.Components;

[RegisterComponent]
public sealed partial class TransferMindOnGibComponent : Component
{
    [DataField]
    public ProtoId<TagPrototype> TargetTag = "MindTransferTarget";
}
