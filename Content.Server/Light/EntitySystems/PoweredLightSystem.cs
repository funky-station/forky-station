// SPDX-FileCopyrightText: 2021-2023, 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021, 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021, 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021, 2023 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2021-2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021-2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Alex Evgrashin <aevgrashin@yandex.ru>
// SPDX-FileCopyrightText: 2021 Kara D <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022-2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022-2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 themias <89101928+themias@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Júlio César Ueti <52474532+Mirino97@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Jack Fox <35575261+DubiousDoggo@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Matz05 <Matz05@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kevin Zheng <kevinz5000@gmail.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 nikthechampiongr <32041239+nikthechampiongr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 LordCarve <27449516+LordCarve@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Server.Ghost;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;

namespace Content.Server.Light.EntitySystems;

/// <summary>
///     System for the PoweredLightComponents
/// </summary>
public sealed class PoweredLightSystem : SharedPoweredLightSystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PoweredLightComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<PoweredLightComponent, GhostBooEvent>(OnGhostBoo);
    }

    private void OnGhostBoo(EntityUid uid, PoweredLightComponent light, GhostBooEvent args)
    {
        if (light.IgnoreGhostsBoo)
            return;

        // check cooldown first to prevent abuse
        var time = GameTiming.CurTime;
        if (light.LastGhostBlink != null)
        {
            if (time <= light.LastGhostBlink + light.GhostBlinkingCooldown)
                return;
        }

        light.LastGhostBlink = time;

        ToggleBlinkingLight(uid, light, true);
        uid.SpawnTimer(light.GhostBlinkingTime, () =>
        {
            ToggleBlinkingLight(uid, light, false);
        });

        args.Handled = true;
    }

    private void OnMapInit(EntityUid uid, PoweredLightComponent light, MapInitEvent args)
    {
        // TODO: Use ContainerFill dog
        if (light.HasLampOnSpawn != null)
        {
            var entity = EntityManager.SpawnEntity(light.HasLampOnSpawn, EntityManager.GetComponent<TransformComponent>(uid).Coordinates);
            ContainerSystem.Insert(entity, light.LightBulbContainer);
        }
        // need this to update visualizers
        UpdateLight(uid, light);
    }
}
