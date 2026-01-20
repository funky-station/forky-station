// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Client.ResourceManagement;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Content.Client.IoC
{
    public static class StaticIoC
    {
        public static IResourceCache ResC => IoCManager.Resolve<IResourceCache>();
    }
}
