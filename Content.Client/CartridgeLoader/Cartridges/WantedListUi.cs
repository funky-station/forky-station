// SPDX-FileCopyrightText: 2024 Эдуард
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed partial class WantedListUi : UIFragment
{
    private WantedListUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _fragment = new WantedListUiFragment();
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        switch (state)
        {
            case WantedListUiState cast:
                _fragment?.UpdateState(cast.Records);
                break;
        }
    }
}
