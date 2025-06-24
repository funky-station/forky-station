// SPDX-FileCopyrightText: 2023-2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023-2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 LankLTE <135308300+LankLTE@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 The Canned One <greentopcan@gmail.com>
// SPDX-FileCopyrightText: 2024 BIGZi0348 <118811750+BIGZi0348@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mervill <mervills.email@gmail.com>
// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mephisto72 <66994453+Mephisto72@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Silicons.Laws;
using Content.Server.StationEvents.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.Silicons.Laws.Components;
using Content.Shared.Station.Components;
using Robust.Shared.Random; // imp

namespace Content.Server.StationEvents.Events;

public sealed class IonStormRule : StationEventSystem<IonStormRuleComponent>
{
    // [Dependency] private readonly IonStormSystem _ionStorm = default!; // imp remove
    [Dependency] private readonly IRobustRandom _random = default!; // imp

    protected override void Started(EntityUid uid, IonStormRuleComponent comp, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, comp, gameRule, args);

        if (!TryGetRandomStation(out var chosenStation))
            return;

        // begin imp edit, why tf wasnt this all just an event
        // var query = EntityQueryEnumerator<SiliconLawBoundComponent, TransformComponent, IonStormTargetComponent>();
        var query = EntityQueryEnumerator<IonStormTargetComponent, TransformComponent>();
        while (query.MoveNext(out var ent, out var target, out var xform))
        // end imp edit
        {
            // only affect law holders on the station, and check random chance (imp edit)
            if (CompOrNull<StationMemberComponent>(xform.GridUid)?.Station != chosenStation ||
                !_random.Prob(target.Chance)) // imp
                continue;
            // begin imp edit again
            var ev = new IonStormEvent();
            RaiseLocalEvent(ent, ref ev);
            //     _ionStorm.IonStormTarget((ent, lawBound, target));
        }
    }
}

// imp add
/// <summary>
/// Event raised on an entity with <see cref="IonStormTargetComponent"/> when an ion storm occurs on the attached station.
/// </summary>
[ByRefEvent]
public record struct IonStormEvent(bool Adminlog = true);
