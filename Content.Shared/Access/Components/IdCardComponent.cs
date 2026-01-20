// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Alfred Baumann
// SPDX-FileCopyrightText: 2025 John
// SPDX-FileCopyrightText: 2024 chavonadelal
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2023 LordEclipse
// SPDX-FileCopyrightText: 2023 PrPleGoo
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 0x6273
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Access.Systems;
using Content.Shared.PDA;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Access.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
[Access(typeof(SharedIdCardSystem), typeof(SharedPdaSystem), typeof(SharedAgentIdCardSystem), Other = AccessPermissions.ReadWrite)]
public sealed partial class IdCardComponent : Component
{
    [DataField]
    [AutoNetworkedField]
    // FIXME Friends
    public string? FullName;

    [DataField]
    [AutoNetworkedField]
    [Access(typeof(SharedIdCardSystem), typeof(SharedPdaSystem), typeof(SharedAgentIdCardSystem), Other = AccessPermissions.ReadWrite)]
    public LocId? JobTitle;

    [DataField]
    [AutoNetworkedField]
    private string? _jobTitle;

    [Access(typeof(SharedIdCardSystem), typeof(SharedPdaSystem), typeof(SharedAgentIdCardSystem), Other = AccessPermissions.ReadWriteExecute)]
    public string? LocalizedJobTitle { set => _jobTitle = value; get => _jobTitle ?? Loc.GetString(JobTitle ?? string.Empty); }

    /// <summary>
    /// The state of the job icon rsi.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public ProtoId<JobIconPrototype> JobIcon = "JobIconUnknown";

    /// <summary>
    /// Holds the job prototype when the ID card has no associated station record
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public ProtoId<JobPrototype>? JobPrototype;

    /// <summary>
    /// The proto IDs of the departments associated with the job
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public List<ProtoId<DepartmentPrototype>> JobDepartments = new();

    /// <summary>
    /// Determines if accesses from this card should be logged by <see cref="AccessReaderComponent"/>
    /// </summary>
    [DataField]
    public bool BypassLogging;

    [DataField]
    public LocId NameLocId = "access-id-card-component-owner-name-job-title-text";

    [DataField]
    public LocId FullNameLocId = "access-id-card-component-owner-full-name-job-title-text";

    [DataField]
    public bool CanMicrowave = true;
}
