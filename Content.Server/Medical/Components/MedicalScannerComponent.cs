// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 Fishfish458
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Morbo
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Wrexbe
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 ShadowCommander
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 SoulSloth
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2020 JiimBob
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Construction.Prototypes;
using Content.Shared.DragDrop;
using Content.Shared.MedicalScanner;
using Robust.Shared.Containers;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Medical.Components
{
    [RegisterComponent]
    public sealed partial class MedicalScannerComponent : SharedMedicalScannerComponent
    {
        public const string ScannerPort = "MedicalScannerReceiver";
        public ContainerSlot BodyContainer = default!;
        public EntityUid? ConnectedConsole;

        [DataField, ViewVariables(VVAccess.ReadWrite)]
        public float CloningFailChanceMultiplier = 1f;
    }
}
