// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Shared.ContentPack;

namespace Content.Shared.Module
{
    public abstract class SharedModuleTestingCallbacks : ModuleTestingCallbacks
    {
        public Action SharedBeforeIoC { get; set; } = default!;
    }
}
