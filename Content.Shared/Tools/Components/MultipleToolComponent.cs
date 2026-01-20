// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared.Tools.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class MultipleToolComponent : Component
{
    [DataDefinition]
    public sealed partial class ToolEntry
    {
        [DataField(required: true)]
        public PrototypeFlags<ToolQualityPrototype> Behavior = new();

        [DataField]
        public SoundSpecifier? UseSound;

        [DataField]
        public SoundSpecifier? ChangeSound;

        [DataField]
        public SpriteSpecifier? Sprite;
    }

    [DataField(required: true)]
    public ToolEntry[] Entries { get; private set; } = Array.Empty<ToolEntry>();

    [ViewVariables]
    [AutoNetworkedField]
    public uint CurrentEntry = 0;

    [ViewVariables]
    public string CurrentQualityName = string.Empty;

    [ViewVariables(VVAccess.ReadWrite)]
    public bool UiUpdateNeeded;

    [DataField]
    public bool StatusShowBehavior = true;
}
