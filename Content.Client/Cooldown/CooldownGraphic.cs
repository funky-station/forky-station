// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Tomeno
// SPDX-FileCopyrightText: 2020 Rich
// SPDX-FileCopyrightText: 2020 Visne
// SPDX-FileCopyrightText: 2020 Tyler Young
// SPDX-License-Identifier: MIT

using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Cooldown
{
    public sealed class CooldownGraphic : Control
    {
        private static readonly ProtoId<ShaderPrototype> Shader = "CooldownAnimation";

        [Dependency] private readonly IGameTiming _gameTiming = default!;
        [Dependency] private readonly IPrototypeManager _protoMan = default!;

        private readonly ShaderInstance _shader;

        public CooldownGraphic()
        {
            IoCManager.InjectDependencies(this);
            _shader = _protoMan.Index(Shader).InstanceUnique();
        }

        /// <summary>
        ///     Progress of the cooldown animation.
        ///     Possible values range from 1 to -1, where 1 to 0 is a depleting circle animation and 0 to -1 is a blink animation.
        /// </summary>
        public float Progress { get; set; }

        protected override void Draw(DrawingHandleScreen handle)
        {
            Span<float> x = new float[10];
            Color color;

            var lerp = 1f - MathF.Abs(Progress); // for future bikeshedding purposes

            if (Progress >= 0f)
            {
                var hue = (5f / 18f) * lerp;
                color = Color.FromHsv(new Vector4(hue, 0.75f, 0.75f, 0.50f));
            }
            else
            {
                var alpha = MathHelper.Clamp(0.5f * lerp, 0f, 0.5f);
                color = new Color(1f, 1f, 1f, alpha);
            }

            _shader.SetParameter("progress", Progress);
            handle.UseShader(_shader);
            handle.DrawRect(PixelSizeBox, color);
            handle.UseShader(null);
        }

        public void FromTime(TimeSpan start, TimeSpan end)
        {
            var duration = end - start;
            var curTime = _gameTiming.CurTime;
            var length = duration.TotalSeconds;
            var progress = (curTime - start).TotalSeconds / length;
            var ratio = (progress <= 1 ? (1 - progress) : (curTime - end).TotalSeconds * -5);

            Progress = MathHelper.Clamp((float) ratio, -1, 1);
            Visible = ratio > -1f;
        }
    }
}
