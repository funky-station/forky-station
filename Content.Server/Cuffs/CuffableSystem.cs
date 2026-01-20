// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 AJCM-git
// SPDX-FileCopyrightText: 2023 DadeKuma
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2022 Errant
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Júlio César Ueti
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 Profane McBane
// SPDX-FileCopyrightText: 2022 KIBORG04
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Wrexbe
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-License-Identifier: MIT

using Content.Shared.Cuffs;
using Content.Shared.Cuffs.Components;
using JetBrains.Annotations;
using Robust.Shared.GameStates;

namespace Content.Server.Cuffs
{
    [UsedImplicitly]
    public sealed class CuffableSystem : SharedCuffableSystem
    {
        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<CuffableComponent, ComponentGetState>(OnCuffableGetState);
        }

        private void OnCuffableGetState(Entity<CuffableComponent> entity, ref ComponentGetState args)
        {
            // there are 2 approaches i can think of to handle the handcuff overlay on players
            // 1 - make the current RSI the handcuff type that's currently active. all handcuffs on the player will appear the same.
            // 2 - allow for several different player overlays for each different cuff type.
            // approach #2 would be more difficult/time consuming to do and the payoff doesn't make it worth it.
            // right now we're doing approach #1.
            HandcuffComponent? cuffs = null;
            if (TryGetLastCuff((entity, entity.Comp), out var cuff))
                TryComp(cuff, out cuffs);
            args.State = new CuffableComponentState(entity.Comp.CuffedHandCount,
                entity.Comp.CanStillInteract,
                cuffs?.CuffedRSI,
                $"{cuffs?.BodyIconState}-{entity.Comp.CuffedHandCount}",
                cuffs?.Color);
            // the iconstate is formatted as blah-2, blah-4, blah-6, etc.
            // the number corresponds to how many hands are cuffed.
        }
    }
}
