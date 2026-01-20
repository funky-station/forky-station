// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Client.Graphics;
using Robust.Shared.Console;

namespace Content.Client.Parallax.Commands;

public sealed class ShowBiomeCommand : LocalizedCommands
{
    [Dependency] private readonly IOverlayManager _overlayMgr = default!;

    public override string Command => "showbiome";
    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (_overlayMgr.HasOverlay<BiomeDebugOverlay>())
        {
            _overlayMgr.RemoveOverlay<BiomeDebugOverlay>();
        }
        else
        {
            _overlayMgr.AddOverlay(new BiomeDebugOverlay());
        }
    }
}
