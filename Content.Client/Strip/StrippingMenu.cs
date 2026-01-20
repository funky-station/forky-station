// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-FileCopyrightText: 2025 LordCarve
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Justin Trotter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Timing;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client.Strip
{
    public sealed class StrippingMenu : DefaultWindow
    {
        public LayoutContainer InventoryContainer = new();
        public LayoutContainer HandsContainer = new();
        public BoxContainer SnareContainer = new();
        public bool Dirty = true;

        public event Action? OnDirty;

        public StrippingMenu()
        {
            var box = new BoxContainer() { Orientation = LayoutOrientation.Vertical, Margin = new Thickness(0, 8) };
            ContentsContainer.AddChild(box);
            box.AddChild(SnareContainer);
            box.AddChild(HandsContainer);
            box.AddChild(InventoryContainer);
        }

        public void ClearButtons()
        {
            InventoryContainer.RemoveAllChildren();
            HandsContainer.RemoveAllChildren();
            SnareContainer.RemoveAllChildren();
        }

        protected override void FrameUpdate(FrameEventArgs args)
        {
            if (!Dirty)
                return;

            Dirty = false;
            OnDirty?.Invoke();
        }
    }
}
