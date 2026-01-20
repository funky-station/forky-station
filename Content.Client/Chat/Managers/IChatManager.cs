// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2021 Leo
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-License-Identifier: MIT

using Content.Shared.Chat;

namespace Content.Client.Chat.Managers
{
    public interface IChatManager : ISharedChatManager
    {
        public void SendMessage(string text, ChatSelectChannel channel);
    }
}
