// SPDX-FileCopyrightText: 2021 20kdc <asdd2808@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 corentt <36075110+corentt@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Vasilis <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 BramvanZijp <56019239+BramvanZijp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Brandon Hu <103440971+Brandon-Huu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 YotaXP <yotaxp+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Kirus59 <145689588+Kirus59@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Power.Components;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Rejuvenate;
using Robust.Shared.Utility;

namespace Content.Server.Power.EntitySystems;

public sealed class BatterySystem : SharedBatterySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PowerNetworkBatteryComponent, RejuvenateEvent>(OnNetBatteryRejuvenate);
        SubscribeLocalEvent<NetworkBatteryPreSync>(PreSync);
        SubscribeLocalEvent<NetworkBatteryPostSync>(PostSync);
    }

    protected override void OnStartup(Entity<BatteryComponent> ent, ref ComponentStartup args)
    {
        // Debug assert to prevent anyone from killing their networking performance by dirtying a battery's charge every single tick.
        // This checks for components that interact with the power network, have a charge rate that ramps up over time and therefore
        // have to set the charge in an update loop instead of using a <see cref="RefreshChargeRateEvent"/> subscription.
        // This is usually the case for APCs, SMES, battery powered turrets or similar.
        // For those entities you should disable net sync for the battery in your prototype, using
        /// <code>
        /// - type: Battery
        ///   netSync: false
        /// </code>
        /// This disables networking and prediction for this battery.
        if (!ent.Comp.NetSyncEnabled)
            return;

        DebugTools.Assert(!HasComp<ApcPowerReceiverBatteryComponent>(ent), $"{ToPrettyString(ent.Owner)} has a predicted battery connected to the power net. Disable net sync!");
        DebugTools.Assert(!HasComp<PowerNetworkBatteryComponent>(ent), $"{ToPrettyString(ent.Owner)} has a predicted battery connected to the power net. Disable net sync!");
        DebugTools.Assert(!HasComp<PowerConsumerComponent>(ent), $"{ToPrettyString(ent.Owner)} has a predicted battery connected to the power net. Disable net sync!");
    }


    private void OnNetBatteryRejuvenate(Entity<PowerNetworkBatteryComponent> ent, ref RejuvenateEvent args)
    {
        ent.Comp.NetworkBattery.CurrentStorage = ent.Comp.NetworkBattery.Capacity;
    }

    private void PreSync(NetworkBatteryPreSync ev)
    {
        // Ignoring entity pausing. If the entity was paused, neither component's data should have been changed.
        var enumerator = AllEntityQuery<PowerNetworkBatteryComponent, BatteryComponent>();
        while (enumerator.MoveNext(out var uid, out var netBat, out var bat))
        {
            var currentCharge = GetCharge((uid, bat));
            DebugTools.Assert(currentCharge <= bat.MaxCharge && currentCharge >= 0);
            netBat.NetworkBattery.Capacity = bat.MaxCharge;
            netBat.NetworkBattery.CurrentStorage = currentCharge;
        }
    }

    private void PostSync(NetworkBatteryPostSync ev)
    {
        // Ignoring entity pausing. If the entity was paused, neither component's data should have been changed.
        var enumerator = AllEntityQuery<PowerNetworkBatteryComponent, BatteryComponent>();
        while (enumerator.MoveNext(out var uid, out var netBat, out var bat))
        {
            SetCharge((uid, bat), netBat.NetworkBattery.CurrentStorage);
        }
    }
}
