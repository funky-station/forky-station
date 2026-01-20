// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 kosticia
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2025 Ciarán Walsh
// SPDX-FileCopyrightText: 2025 Ignaz "Ian" Kraft
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 chavonadelal
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2024 BombasterDS
// SPDX-FileCopyrightText: 2024 eoineoineoin
// SPDX-FileCopyrightText: 2024 Pspritechologist
// SPDX-FileCopyrightText: 2024 Jake Huxell
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2023 Julian Giebel
// SPDX-FileCopyrightText: 2024 Julian Giebel
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 themias
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2023 Ahion
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2022 KIBORG04
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Content.Server.DeviceNetwork.Systems;
using Content.Server.Medical.CrewMonitoring;
using Content.Shared.DeviceNetwork.Components;
using Content.Shared.Medical.SuitSensors;
using Robust.Shared.Timing;

namespace Content.Server.Medical.SuitSensors;

public sealed class SuitSensorSystem : SharedSuitSensorSystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly DeviceNetworkSystem _deviceNetworkSystem = default!;
    [Dependency] private readonly SingletonDeviceNetServerSystem _singletonServerSystem = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _gameTiming.CurTime;
        var sensors = EntityQueryEnumerator<SuitSensorComponent, DeviceNetworkComponent>();

        while (sensors.MoveNext(out var uid, out var sensor, out var device))
        {
            if (device.TransmitFrequency is null)
                continue;

            // check if sensor is ready to update
            if (curTime < sensor.NextUpdate)
                continue;
            sensor.NextUpdate += sensor.UpdateRate;

            if (!CheckSensorAssignedStation((uid, sensor)))
                continue;

            // get sensor status
            var status = GetSensorState((uid, sensor));
            if (status == null)
                continue;

            //Retrieve active server address if the sensor isn't connected to a server
            if (sensor.ConnectedServer == null)
            {
                if (!_singletonServerSystem.TryGetActiveServerAddress<CrewMonitoringServerComponent>(sensor.StationId!.Value, out var address))
                    continue;

                sensor.ConnectedServer = address;
            }

            // Send it to the connected server
            var payload = SuitSensorToPacket(status);

            // Clear the connected server if its address isn't on the network
            if (!_deviceNetworkSystem.IsAddressPresent(device.DeviceNetId, sensor.ConnectedServer))
            {
                sensor.ConnectedServer = null;
                continue;
            }

            _deviceNetworkSystem.QueuePacket(uid, sensor.ConnectedServer, payload, device: device);
        }
    }
}
