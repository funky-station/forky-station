// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Client.UserInterface.Systems;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls
{
    public sealed class ProgressTextureRect : TextureRect
    {
        public float Progress;

        private readonly ProgressColorSystem _progressColor;

        public ProgressTextureRect()
        {
            _progressColor = IoCManager.Resolve<IEntityManager>().System<ProgressColorSystem>();
        }

        protected override void Draw(DrawingHandleScreen handle)
        {
            var dims = Texture != null ? GetDrawDimensions(Texture) : UIBox2.FromDimensions(Vector2.Zero, PixelSize);
            dims.Top = Math.Max(dims.Bottom - dims.Bottom * Progress,0);
            handle.DrawRect(dims, _progressColor.GetProgressColor(Progress));

            base.Draw(handle);
        }
    }
}
