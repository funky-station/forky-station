// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Shuttles.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates;

[Serializable, NetSerializable]
public sealed class IFFConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
    public IFFFlags AllowedFlags;
    public IFFFlags Flags;
}

[Serializable, NetSerializable]
public enum IFFConsoleUiKey : byte
{
    Key,
}
