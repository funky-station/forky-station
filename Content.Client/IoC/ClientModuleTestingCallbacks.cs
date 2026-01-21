// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using System;
using Content.Shared;
using Content.Shared.Module;

namespace Content.Client.IoC
{
    public sealed class ClientModuleTestingCallbacks : SharedModuleTestingCallbacks
    {
        public Action? ClientBeforeIoC { get; set; }
    }
}
