// SPDX-FileCopyrightText: 2025 ElectroJr
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Moony
// SPDX-License-Identifier: MIT

using Content.Shared.Administration;
using Robust.Shared.Toolshed;

namespace Content.Server.Administration.Toolshed;

[ToolshedCommand, AnyCommand]
public sealed class MarkedCommand : ToolshedCommand
{
    [CommandImplementation]
    public IEnumerable<EntityUid> Marked(IInvocationContext ctx)
    {
        var marked = ctx.ReadVar("marked") as IEnumerable<EntityUid>;
        return marked ?? Array.Empty<EntityUid>();
    }
}
