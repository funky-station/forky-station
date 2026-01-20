// SPDX-FileCopyrightText: 2025 deltanedas
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed partial class LogProbeUi : UIFragment
{
    private LogProbeUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface ui, EntityUid? fragmentOwner)
    {
        _fragment = new LogProbeUiFragment();

        _fragment.OnPrintPressed += () =>
        {
            var ev = new LogProbePrintMessage();
            var message = new CartridgeUiMessage(ev);
            ui.SendMessage(message);
        };
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is not LogProbeUiState cast)
            return;

        _fragment?.UpdateState(cast.EntityName, cast.PulledLogs);
    }
}
