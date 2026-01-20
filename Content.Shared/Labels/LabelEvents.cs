// SPDX-FileCopyrightText: 2024 osjarw
// SPDX-FileCopyrightText: 2024 Verm
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Watermelon914
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Labels;

/// <summary>
/// Key representing which <see cref="PlayerBoundUserInterface"/> is currently open.
/// Useful when there are multiple UI for an object. Here it's future-proofing only.
/// </summary>
[Serializable, NetSerializable]
public enum HandLabelerUiKey
{
    Key,
}

[Serializable, NetSerializable]
public enum PaperLabelVisuals : byte
{
    Layer,
    HasLabel,
    LabelType
}

[Serializable, NetSerializable]
public sealed class HandLabelerLabelChangedMessage(string label) : BoundUserInterfaceMessage
{
    public string Label { get; } = label;
}
