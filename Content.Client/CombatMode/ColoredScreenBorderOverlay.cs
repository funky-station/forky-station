// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 GraniteSidewalk
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2020 R. Neuser
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2018 clusterfack
// SPDX-License-Identifier: MIT

using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.CombatMode
{
    public sealed class ColoredScreenBorderOverlay : Overlay
    {
        private static readonly ProtoId<ShaderPrototype> Shader = "ColoredScreenBorder";

        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public override OverlaySpace Space => OverlaySpace.WorldSpace;
        private readonly ShaderInstance _shader;

        public ColoredScreenBorderOverlay()
        {
            IoCManager.InjectDependencies(this);
            _shader = _prototypeManager.Index(Shader).Instance();
        }

        protected override void Draw(in OverlayDrawArgs args)
        {
            var worldHandle = args.WorldHandle;
            worldHandle.UseShader(_shader);
            var viewport = args.WorldAABB;
            worldHandle.DrawRect(viewport, Color.White);
            worldHandle.UseShader(null);
        }
    }
}
