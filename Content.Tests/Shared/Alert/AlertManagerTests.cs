// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-License-Identifier: MIT

using System.IO;
using Content.Client.Alerts;
using Content.Shared.Alert;
using NUnit.Framework;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Utility;
using Robust.UnitTesting;

namespace Content.Tests.Shared.Alert
{
    [TestFixture, TestOf(typeof(AlertsSystem))]
    public sealed class AlertManagerTests : RobustUnitTest
    {
        const string PROTOTYPES = @"
- type: alert
  id: LowPressure
  icons:
  - /Textures/Interface/Alerts/Pressure/lowpressure.png

- type: alert
  id: HighPressure
  icons:
  - /Textures/Interface/Alerts/Pressure/highpressure.png
";

        [Test]
        [Ignore("There is no way to load extra Systems in a unit test, fixing RobustUnitTest is out of scope.")]
        public void TestAlertManager()
        {
            var entManager = IoCManager.Resolve<IEntityManager>();
            var sysManager = entManager.EntitySysManager;
            sysManager.LoadExtraSystemType<ClientAlertsSystem>();
            var alertsSystem = sysManager.GetEntitySystem<ClientAlertsSystem>();
            IoCManager.Resolve<ISerializationManager>().Initialize();

            var reflection = IoCManager.Resolve<IReflectionManager>();
            reflection.LoadAssemblies();

            var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
            prototypeManager.Initialize();
            prototypeManager.LoadFromStream(new StringReader(PROTOTYPES));

            Assert.That(alertsSystem.TryGet("LowPressure", out var lowPressure));
            Assert.That(lowPressure!.Icons[0], Is.EqualTo(new SpriteSpecifier.Texture(new ("/Textures/Interface/Alerts/Pressure/lowpressure.png"))));
            Assert.That(alertsSystem.TryGet("HighPressure", out var highPressure));
            Assert.That(highPressure!.Icons[0], Is.EqualTo(new SpriteSpecifier.Texture(new ("/Textures/Interface/Alerts/Pressure/highpressure.png"))));

            Assert.That(alertsSystem.TryGet("LowPressure", out lowPressure));
            Assert.That(lowPressure!.Icons[0], Is.EqualTo(new SpriteSpecifier.Texture(new ("/Textures/Interface/Alerts/Pressure/lowpressure.png"))));
            Assert.That(alertsSystem.TryGet("HighPressure", out highPressure));
            Assert.That(highPressure!.Icons[0], Is.EqualTo(new SpriteSpecifier.Texture(new ("/Textures/Interface/Alerts/Pressure/highpressure.png"))));
        }
    }
}
