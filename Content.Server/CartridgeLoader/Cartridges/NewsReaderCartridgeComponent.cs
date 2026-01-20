// SPDX-FileCopyrightText: 2024 Julian Giebel
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 MishaUnity
// SPDX-License-Identifier: MIT

namespace Content.Server.CartridgeLoader.Cartridges;

[RegisterComponent]
public sealed partial class NewsReaderCartridgeComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    public int ArticleNumber;

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public bool NotificationOn = true;
}
