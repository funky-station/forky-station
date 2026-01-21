// SPDX-FileCopyrightText: 2023 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Client.Gameplay;
using Robust.Client.UserInterface.Controllers;

namespace Content.Client.UserInterface.Systems.Gameplay;

public sealed class GameplayStateLoadController : UIController
{
    public Action? OnScreenLoad;
    public Action? OnScreenUnload;

    public void UnloadScreen()
    {
        OnScreenUnload?.Invoke();
    }

    public void LoadScreen()
    {
        OnScreenLoad?.Invoke();
    }
}
