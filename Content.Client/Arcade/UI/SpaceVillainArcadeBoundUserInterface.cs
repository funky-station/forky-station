// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;
using static Content.Shared.Arcade.SharedSpaceVillainArcadeComponent;

namespace Content.Client.Arcade.UI;

public sealed class SpaceVillainArcadeBoundUserInterface : BoundUserInterface
{
    [ViewVariables] private SpaceVillainArcadeMenu? _menu;

    public SpaceVillainArcadeBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        SendAction(PlayerAction.RequestData);
    }

    public void SendAction(PlayerAction action)
    {
        SendMessage(new SpaceVillainArcadePlayerActionMessage(action));
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<SpaceVillainArcadeMenu>();
        _menu.OnPlayerAction += SendAction;
    }

    protected override void ReceiveMessage(BoundUserInterfaceMessage message)
    {
        if (message is SpaceVillainArcadeDataUpdateMessage msg)
            _menu?.UpdateInfo(msg);
    }
}
